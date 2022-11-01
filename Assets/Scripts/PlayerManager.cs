using Newtonsoft.Json;
using RiptideNetworking;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static Dictionary<ushort, PlayerData> playerList = new Dictionary<ushort, PlayerData>();

    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "lobby" && PlayerPrefs.GetString("multiPlayerMode") == "server"){
            PlayerData playerData = FindObjectOfType<AvatarManager>().GetPlayerData();
            SpawnPlayer(0, playerData); //0 é o servidor
            FindObjectOfType<GUILobbyManager>()?.UpdatePlayers(playerList);
        }
    }

    public void RemovePlayerFromList(ushort playerId)
    {
        if (playerList.ContainsKey(playerId))
        {
            if(SceneManager.GetActiveScene().name == "lobby")
                playerList.Remove(playerId);
            else
                playerList[playerId].active = false;
            FindObjectOfType<GUILobbyManager>().UpdatePlayers(playerList);
        }
    }

    public void SetPlayersData(Dictionary<ushort, PlayerData> playerListReceived)
    {
        playerList = playerListReceived;
        FindObjectOfType<GUILobbyManager>().UpdatePlayers(playerList);
    }

    public static void SpawnPlayer(ushort id, PlayerData playerData)
    {
        if (!playerList.ContainsKey(id))
        {
            playerData.id = id;
            playerData.palavraAtual = 0;
            playerData.qtdTentativas = 0;
            playerData.active = true;
            
            playerList.Add(id, playerData);
            Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerStats);
            message.AddString(JsonConvert.SerializeObject(playerList));
            foreach (PlayerData otherPlayer in playerList.Values)
                if(otherPlayer.id != 0)
                    NetworkServerManager.Singleton.Server.Send(message, otherPlayer.id);
        }
    }

    #region Messages
    [MessageHandler((ushort)ClientToServerId.playerDataMsg)]
    private static void GetPlayerData(ushort fromClientId, Message message)
    {
        if (FindObjectOfType<MultiPlayerServerGuessesManager>() == null || !FindObjectOfType<MultiPlayerServerGuessesManager>().timerStarted) //in lobby
        {
            string messageStr = message.GetString();
            Debug.Log($"Mensagem do cliente {fromClientId}: {messageStr}");
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(messageStr);
            SpawnPlayer(fromClientId, playerData);
            FindObjectOfType<GUILobbyManager>().UpdatePlayers(playerList);
        }
        else
        {
            NetworkServerManager.Singleton.Server.DisconnectClient(fromClientId);
        }
    }

    public static string GetGuessFromPlayer(ushort fromClientId, string attempt)
    {
        string checkedAttempt = FindObjectOfType<MultiPlayerServerGuessesManager>().GetCheckedAttemptOfUser(attempt, playerList[fromClientId].palavraAtual);
        Debug.Log($"Tentativa da palavra {playerList[fromClientId].palavraAtual} do cliente {fromClientId} - {attempt} = {checkedAttempt}");

        if (checkedAttempt == "X" || checkedAttempt.Contains((char)AttempededLetter.Missed) || checkedAttempt.Contains((char)AttempededLetter.NotInWord))
        {
            playerList[fromClientId].qtdTentativas++;
        }
        else
        {
            Debug.Log($"Player {fromClientId} guessed right");
            playerList[fromClientId].score += Math.Max(0,  1100 - (playerList[fromClientId].qtdTentativas * 100));
            playerList[fromClientId].qtdTentativas = 0;
            playerList[fromClientId].palavraAtual++;
        }
        return checkedAttempt;
    }

    [MessageHandler((ushort)ClientToServerId.wordGuess)]
    private static void GetGuessFromPlayer(ushort fromClientId, Message message)
    {
        if (FindObjectOfType<MultiPlayerServerGuessesManager>().timerStarted)
        {
            if (playerList.ContainsKey(fromClientId))
            {
                string messageStr = message.GetString();
                Debug.Log($"Mensagem do cliente {fromClientId}: {messageStr}");

                Message newMessage = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.wordAnswer);
                newMessage.AddString(GetGuessFromPlayer(fromClientId, messageStr));
                NetworkServerManager.Singleton.Server.Send(newMessage, fromClientId);

                if (playerList[fromClientId].palavraAtual >= FindObjectOfType<MultiPlayerServerGuessesManager>().qtdPalavrasJogo)
                {
                    Debug.Log($"GAME OVER - Player {fromClientId} won!");
                    Message gameOverMessage2 = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.gameOver);
                    gameOverMessage2.AddUShort(fromClientId);
                    NetworkServerManager.Singleton.Server.SendToAll(gameOverMessage2);
                    NetworkServerManager.Singleton.Server.Stop();
                    //to winning screen

                }
            }
        }
        else
        {
            NetworkServerManager.Singleton.Server.DisconnectClient(fromClientId);
        }
    }
    #endregion
}
