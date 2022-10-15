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

    #region Messages

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
        FindObjectOfType<WordManager>().ReceiveAttemptedLetters(messageStr);
    }

    [MessageHandler((ushort)ServerToClientId.serverQuitGame)]
    private static void ServerQuitGame(Message message)
    {
        Debug.Log($"The game has been ended by the server.");
    }

    [MessageHandler((ushort)ServerToClientId.playerStats)]
    private static void NewPlayerJoined(Message message)
    {
        string playersData = message.GetString();
        FindObjectOfType<PlayerManager>().SetPlayersData(JsonUtility.FromJson<Dictionary<ushort, PlayerData>> (playersData));
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
