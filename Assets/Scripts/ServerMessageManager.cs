using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;
//using Unity.Netcode;

public class ServerMessageManager : MonoBehaviour
{
    public static Dictionary<ushort, ServerMessageManager> playerList = new Dictionary<ushort, ServerMessageManager>();

    public ushort Id { get; private set; }
    public string Username { get; private set; }

    private void Start()
    {
        Debug.Log("ServerMessageManager - hii");
    }
    private void OnDestroy()
    {
        //playerList.Remove(Id);
    }

    public static void Spawn(ushort id, string username)
    {
        foreach (ServerMessageManager otherPlayer in playerList.Values)
            otherPlayer.SendSpawned(id);

        //PlayerManager player = Instantiate(GameLogic.Singleton.PlayerPrefab, new Vector3(0f, 1f, 0f), Quaternion.identity).GetComponent<Player>();
        //player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        //player.Id = id;
        //player.Username = string.IsNullOrEmpty(username) ? $"Guest {id}" : username;

        //player.SendSpawned();
        //playerList.Add(id, player);
    }

    #region Messages
    private void SendSpawned()
    {
        //NetworkServerManager.Singleton.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerSpawned)));
    }

    private void SendSpawned(ushort toClientId)
    {
        //NetworkServerManager.Singleton.Server.Send(AddSpawnData(Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerSpawned)), toClientId);
    }

    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Id);
        message.AddString(Username);
        //message.AddVector3(transform.position);
        return message;
    }

    private static Message AddGuessCheck(Message message, string attempt)
    {
        SinglePlayerGuessesManager guessesManager = new SinglePlayerGuessesManager();
        message.AddString(guessesManager.GetCheckedAttempt(attempt));
        return message;
    }

    [MessageHandler((ushort)ServerToClientId.wordAnswer)]
    private static void GetGuessFromServer(Message message)
    {
        string messageStr = message.GetString();
        Debug.Log($"Mensagem do servidor: {messageStr}");
        //WordManager wordManager = new WordManager();
        //wordManager.ReceiveAttemptedLetters(messageStr);
        FindObjectOfType<WordManager>().ReceiveAttemptedLetters(messageStr);
        //NetworkServerManager.Singleton.Server.Send(AddGuessCheck(Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerSpawned), messageStr), fromClientId);

        //return message;
        //playerList.Add(fromClientId, new PlayerManager);
    }

    [MessageHandler((ushort)ServerToClientId.serverQuitGame)]
    private static void ServerQuitGame(Message message)
    {
        Debug.Log($"The game has been ended by the server.");
    }

    [MessageHandler((ushort)ServerToClientId.playerStats)]
    private static void NewPlayerJoined(Message message)
    {
        Debug.Log($"A player joined the game.");
    }

    [MessageHandler((ushort)ServerToClientId.youLost)]
    private static void PlayerLost(Message message)
    {
        ushort quittingPlayer = message.GetUShort();
        Debug.Log($"Player {quittingPlayer} left the game.");
        //destroy player data
    }

    [MessageHandler((ushort)ServerToClientId.gameOver)]
    private static void GameOver(Message message)
    {
        ushort winningPlayer = message.GetUShort();
        Debug.Log($"GAME OVER - Player {winningPlayer} WON!");
        //destroy everyone's data, go back to menu
    }
    #endregion
}
