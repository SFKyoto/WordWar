using RiptideNetworking;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static Dictionary<ushort, PlayerData> playerList = new Dictionary<ushort, PlayerData>();

    private void Start()
    {
        FindObjectOfType<GUILobbyManager>().SetIPTXT();

        PlayerData playerData = FindObjectOfType<AvatarManager>().GetPlayerData();
        SpawnPlayer(0, playerData);
        FindObjectOfType<GUILobbyManager>().UpdatePlayers(playerList);

        //NetworkServerManager.Singleton.Server.ClientDisconnected += new EventHandler(s, e) => RemovePlayerFromList(e.Id);
    }

    private void RemovePlayerFromList(ushort playerId)
    {
        playerList.Remove(playerId);
    }

    private void OnDestroy()
    {
        //Debug.Log(Id.ToString() + " Sent bye");
        //playerList.Remove(Id);
    }

    public static void SpawnPlayer(ushort id, PlayerData playerData)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerStats);
        message.AddString(JsonUtility.ToJson(playerData));
        foreach (PlayerData otherPlayer in playerList.Values)
            NetworkServerManager.Singleton.Server.Send(message, otherPlayer.Id);

        playerData.Id = id;
        playerData.PalavraAtual = 0;
        playerData.QtdTentativas = 0;

        playerList.Add(id, playerData);
    }

    #region Messages
    public static string GetGuessFromPlayer(ushort fromClientId, string attempt)
    {
        //if (!playerList.ContainsKey(fromClientId))
        //    SpawnPlayer(fromClientId, "Player");

        string checkedAttempt = FindObjectOfType<MultiPlayerServerGuessesManager>().GetCheckedAttemptOfUser(attempt, playerList[fromClientId].PalavraAtual);
        Debug.Log($"Tentativa da palavra {playerList[fromClientId].PalavraAtual} do cliente {fromClientId} - {attempt} = {checkedAttempt}");

        if (checkedAttempt == "X" || checkedAttempt.Contains((char)AttempededLetter.Missed) || checkedAttempt.Contains((char)AttempededLetter.NotInWord))
        {
            playerList[fromClientId].QtdTentativas++;
        }
        else
        {
            Debug.Log($"Player {fromClientId} guessed right");
            playerList[fromClientId].QtdTentativas = 0;
            playerList[fromClientId].PalavraAtual++;
        }
        return checkedAttempt;
    }

    [MessageHandler((ushort)ClientToServerId.playerDataMsg)]
    private static void GetPlayerData(ushort fromClientId, Message message)
    {
        if (!FindObjectOfType<MultiPlayerServerGuessesManager>().timerStarted)
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

    [MessageHandler((ushort)ClientToServerId.wordGuess)]
    private static void GetGuessFromPlayer(ushort fromClientId, Message message)
    {
        if (FindObjectOfType<MultiPlayerServerGuessesManager>().timerStarted)
        {
            string messageStr = message.GetString();
            Debug.Log($"Mensagem do cliente {fromClientId}: {messageStr}");

            Message newMessage = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.wordAnswer);
            newMessage.AddString(GetGuessFromPlayer(fromClientId, messageStr));
            NetworkServerManager.Singleton.Server.Send(newMessage, fromClientId);

            if (playerList[fromClientId].PalavraAtual >= FindObjectOfType<MultiPlayerServerGuessesManager>().qtdPalavrasJogo)
            {
                Debug.Log($"GAME OVER - Player {fromClientId} won!");
                Message gameOverMessage2 = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.gameOver);
                gameOverMessage2.AddUShort(fromClientId);
                NetworkServerManager.Singleton.Server.SendToAll(gameOverMessage2);
                NetworkServerManager.Singleton.Server.Stop();
                //to winning screen

                //foreach (KeyValuePair<ushort, PlayerManager> player in PlayerManager.playerList)
                //{
                //    Message gameOverMessage = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.gameOver);
                //    gameOverMessage.AddUShort(player.Value.Id);
                //    NetworkServerManager.Singleton.Server.Send(gameOverMessage, 0);
                //    NetworkServerManager.Singleton.Server.DisconnectClient(player.Value.Id);
                //}
            }
        }
    }
    #endregion
}
