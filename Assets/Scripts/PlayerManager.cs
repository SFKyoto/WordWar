using Newtonsoft.Json;
using RiptideNetworking;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour
{
    public static Dictionary<ushort, PlayerData> playerList = new Dictionary<ushort, PlayerData>();
    public static string multiPlayerMode;
    public static bool isInLobby = true;
    public static readonly int tiedMatch = 404;
    public static bool isThereWinningPlayer;
    public static ushort winningPlayer;

    private void Start()
    {
        //Debug.Log("isInLobby hasValue agr: " + PlayerManager.isInLobby.HasValue);
        //isInLobby = isInLobby.HasValue ? isInLobby : true;
        if(isInLobby && multiPlayerMode == "server"){
            PlayerData playerData = FindObjectOfType<AvatarManager>().GetPlayerData();
            SpawnPlayer(0, playerData); //0 é o servidor
            FindObjectOfType<GUILobbyManager>()?.UpdatePlayers(playerList);
        }
    }

    public void RemovePlayerFromList(ushort playerId)
    {
        if (playerList.ContainsKey(playerId))
        {
            if(isInLobby)
            {
                playerList.Remove(playerId);
                FindObjectOfType<GUILobbyManager>().UpdatePlayers(playerList);
            }
            else
            {
                playerList[playerId].active = false;
                FindObjectOfType<GUIMultiplayerManager>().UpdatePlayers(playerList);
            }
        }
    }

    /// <summary>
    /// Checa jogadores ativos restantes e encerra a partida caso não existam mais.
    /// </summary>
    public void CheckRemainingPlayers()
    {
        var activePlayers = playerList.Where(player => player.Value.active);
        if (activePlayers.Count() <= 1)
        {
            Message gameOverMessage = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.gameOver);
            ushort winningPlayer = (ushort)(activePlayers.Count() <= 0 ? tiedMatch : activePlayers.First().Key);
            gameOverMessage.AddUShort(winningPlayer);
            NetworkServerManager.Singleton.Server.SendToAll(gameOverMessage);
            NetworkServerManager.Singleton.Server.Stop();

            //para tela de vitória
            SendWinningPlayer(winningPlayer);
        }
    }

    public void SetPlayersData(Dictionary<ushort, PlayerData> playerListReceived)
    {
        playerList = playerListReceived;
        try { FindObjectOfType<GUILobbyManager>().UpdatePlayers(playerList); }
        catch { }
        try { FindObjectOfType<GUIMultiplayerManager>().UpdatePlayers(playerList); }
        catch { }
    }

    public void SendWinningPlayer(ushort winningPlayer)
    {
        try { FindObjectOfType<MultiPlayerGuessesManager>().timerStarted = false; }
        catch { }
        if (winningPlayer == tiedMatch && playerList.Count == 1) winningPlayer = 0; //o próprio servidor
        Debug.Log("fim de jogo - winning player: " + winningPlayer);
        FindObjectOfType<WordManager>().BecomeObserver();
        try { FindObjectOfType<MultiPlayerServerGuessesManager>().timerStarted = false; }
        catch { }
        PlayerManager.winningPlayer = winningPlayer;
        PlayerManager.isThereWinningPlayer = true;
        MainMenu.onMainMenuPressed();
        //FindObjectOfType<GUIMultiplayerManager>().ShowWinningPlayer(winningPlayer);
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

    public void SendMessageToAll(Message message)
    {
        foreach (PlayerData otherPlayer in playerList.Values)
            if (otherPlayer.id != 0)
                NetworkServerManager.Singleton.Server.Send(message, otherPlayer.id);
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

        playerList[fromClientId].qtdTentativas++;
        if (!(checkedAttempt == "X" || checkedAttempt.Contains((char)AttempededLetter.Missed) || checkedAttempt.Contains((char)AttempededLetter.NotInWord)))
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
        Debug.Log("getguessfromplayer, timeStarted: " + FindObjectOfType<MultiPlayerServerGuessesManager>().timerStarted);
        if (FindObjectOfType<MultiPlayerServerGuessesManager>().timerStarted)
        {
            if (playerList.ContainsKey(fromClientId) && playerList[fromClientId].active)
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
                    FindObjectOfType<WordManager>().BecomeObserver();
                    FindObjectOfType<MultiPlayerServerGuessesManager>().timerStarted = false;
                    FindObjectOfType<GUIMultiplayerManager>().ShowWinningPlayer(fromClientId);
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
