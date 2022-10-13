using RiptideNetworking;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//public class PlayerStat
//{
//    public PlayerStat(ushort id)
//    {
//        Id = id;
//    }

//    public ushort Id { get; private set; }
//    public string Username { get; private set; }
//    public ushort Avatar { get; private set; }
//    public ushort PalavraAtual { get; private set; }
//    public ushort QtdTentativas { get; private set; }
//}

public class PlayerManager : MonoBehaviour
{
    public static Dictionary<ushort, PlayerManager> playerList = new Dictionary<ushort, PlayerManager>();

    public ushort Id { get; private set; }
    public string Username { get; private set; }
    public BodyPartList Avatar { get; private set; }
    public ushort PalavraAtual { get; private set; }
    public ushort QtdTentativas { get; private set; }

    private void Start()
    {
        Debug.Log(FindObjectOfType<GUILobbyManager>());
        try
        {
            FindObjectOfType<GUILobbyManager>().SetIPTXT();
        }
        catch(Exception e)
        {
        }

    }
    private void OnDestroy()
    {
        Debug.Log(Id.ToString() + " Sent bye");
        playerList.Remove(Id);
    }

    public static void SpawnPlayer(ushort id, PlayerData playerData)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerStats);
        message.AddString(JsonUtility.ToJson(playerData));
        foreach (PlayerManager otherPlayer in playerList.Values)
            NetworkServerManager.Singleton.Server.Send(message, otherPlayer.Id);

        PlayerManager playerStat = new PlayerManager
        {
            Id = id,
            Username = playerData.username,
            Avatar = playerData.bodyPartList,
            PalavraAtual = 0,
            QtdTentativas = 0
        };

        playerList.Add(id, playerStat);
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
