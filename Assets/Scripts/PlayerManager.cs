using RiptideNetworking;
using System.Collections.Generic;
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
    public ushort Avatar { get; private set; }
    public ushort PalavraAtual { get; private set; }
    public ushort QtdTentativas { get; private set; }
    //SinglePlayerGuessesManager guessesManager = new SinglePlayerGuessesManager();


    private void Start()
    {

    }
    private void OnDestroy()
    {
        playerList.Remove(Id);
    }

    public static void SpawnPlayer(ushort id, string username)
    {
        //foreach (PlayerStat otherPlayer in playerList.Values)
        //    SendSpawned(otherPlayer);

        PlayerManager playerStat = new PlayerManager
        {
            Id = id,
            Username = username,
            PalavraAtual = 0,
            QtdTentativas = 0
        };

        playerList.Add(id, playerStat);
    }

    #region Messages
    private void SendSpawned()
    {
        NetworkServerManager.Singleton.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.wordAnswer)));
    }

    private void SendSpawned(ushort toClientId)
    {
        NetworkServerManager.Singleton.Server.Send(AddSpawnData(Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.wordAnswer)), toClientId);
    }

    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Id);
        message.AddString(Username);
        //message.AddVector3(transform.position);
        return message;
    }

    public static string GetGuessFromPlayer(ushort fromClientId, string attempt)
    {
        if (!playerList.ContainsKey(fromClientId))
            SpawnPlayer(fromClientId, "Player");

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
                Debug.Log($"GAME OVER - Player 0 won!");

                foreach (KeyValuePair<ushort, PlayerManager> player in PlayerManager.playerList)
                {
                    Message gameOverMessage = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.gameOver);
                    gameOverMessage.AddUShort(player.Value.Id);
                    NetworkServerManager.Singleton.Server.Send(gameOverMessage, 0);
                    NetworkServerManager.Singleton.Server.DisconnectClient(player.Value.Id);
                }
            }
        }
    }
    #endregion
}
