using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using RiptideNetworking;
using System;

public class MultiPlayerServerGuessesManager : GameGuessesManager
{
    //public NetworkClientManager networkClientManager;

    [Header("Game settings")]
    public ushort qtdPalavrasJogo;
    public List<string> palavrasDoJogo;
    public float timeBetweenGuessedWords;

    [Header("Round State")]
    public Boolean timerStarted;
    public float timeLeftBetweenWords;
    public ushort barrierProgress;

    private void Start()
    {
        Debug.Log("multiplayer server started.");
        if (listPossibleAnswers.Count == 0) GetWordLists();
        if (qtdPalavrasJogo <= 0) qtdPalavrasJogo = 2;
        for(int i = 0; i < qtdPalavrasJogo; i++)
        {
            string answer = GetRandomAnswerFromList();
            if (!palavrasDoJogo.Contains(answer) || listPossibleAnswers.Count < qtdPalavrasJogo)
            {
                palavrasDoJogo.Add(answer);
                Debug.Log($"Added {answer}");
            }
            else
                i--;
        }
        timerStarted = false;
        if (timeBetweenGuessedWords <= 0)
            timeBetweenGuessedWords = 30.0f;
        //NetworkClientManager.Singleton.SetSocket("127.0.0.1", 1237);
        //NetworkClientManager.Singleton.StartConn();

        //networkClientManager = new NetworkClientManager();
        //networkClientManager.SetSocket("127.0.0.1", 1237);
        //networkClientManager.StartConn();
    }

    void Update()
    {
        if (timerStarted)
        {
            timeLeftBetweenWords -= Time.deltaTime;
            if (timeLeftBetweenWords <= 0)
            {
                barrierProgress++;
                Debug.Log(PlayerManager.playerList.Count);
                foreach (KeyValuePair<ushort, PlayerManager> player in PlayerManager.playerList)
                {
                    Debug.Log($"{player.Value.Id} -> {player.Value.PalavraAtual}");
                    if(player.Value.PalavraAtual < barrierProgress)
                    {
                        Debug.Log($"Should disconnect user {player.Value.Id}");
                        //disconnect a user, barrier got him
                        Message playerLostMessage = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.youLost);
                        //newMessage.AddUShort(player.Value.Id);
                        NetworkServerManager.Singleton.Server.Send(playerLostMessage, player.Value.Id);
                        NetworkServerManager.Singleton.Server.DisconnectClient(player.Value.Id);
                        //foreach (KeyValuePair<ushort, PlayerManager> playerToDisconnect in PlayerManager.playerList)
                        //{
                        //    Message newMessage = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.youLost);
                        //    newMessage.AddUShort(player.Value.Id);
                        //    NetworkServerManager.Singleton.Server.Send(newMessage, playerToDisconnect.Value.Id);
                        //    NetworkServerManager.Singleton.Server.DisconnectClient(playerToDisconnect.Value.Id);
                        //}
                        //PlayerManager.playerList.Remove(player.Key);
                    }
                }
                timeLeftBetweenWords = timeBetweenGuessedWords;
            }

            Message playerStatsMsg = Message.Create(MessageSendMode.unreliable, (ushort)ServerToClientId.playerStats);
            playerStatsMsg.AddString(JsonUtility.ToJson(PlayerManager.playerList));
            NetworkServerManager.Singleton.Server.SendToAll(playerStatsMsg);
        }
    }

    public override void GenerateNewAnswer()
    {
        if (listPossibleAnswers.Count == 0) GetWordLists();
        
        answerOfTurn = GetRandomAnswerFromList();
        answerOfTurnNoAccents = SinglePlayerTextManipulation.RemoveAccents(answerOfTurn);
    }

    public string GetCheckedAttemptOfUser(string currentGuess, ushort wordPosition)
    {
        Debug.Log(palavrasDoJogo[0]);
        Debug.Log(wordPosition);
        Debug.Log(palavrasDoJogo[wordPosition]);
        return CompareWordWithGuess(currentGuess, palavrasDoJogo[wordPosition]);
    }

    public override string GetCheckedAttempt(string currentGuess)
    {
        string returnedAttempt = PlayerManager.GetGuessFromPlayer(0, currentGuess);

        if (PlayerManager.playerList[0].PalavraAtual >= qtdPalavrasJogo)
        {
            Debug.Log($"GAME OVER - Player 0 won!");
            Message gameOverMessage2 = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.gameOver);
            gameOverMessage2.AddUShort(0);
            NetworkServerManager.Singleton.Server.SendToAll(gameOverMessage2);
            NetworkServerManager.Singleton.Server.Stop();
            //go to result screen

            //foreach (KeyValuePair<ushort, PlayerManager> player in PlayerManager.playerList)
            //{
            //    if (player.Value.PalavraAtual < barrierProgress)
            //    {
            //        //you (player 0) won!

            //        //foreach (KeyValuePair<ushort, PlayerManager> player2 in PlayerManager.playerList)
            //        //{
            //        //    Message newMessage = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.gameOver);
            //        //    newMessage.AddUShort(player.Value.Id);
            //        //    NetworkServerManager.Singleton.Server.Send(newMessage, player2.Value.Id);
            //        //    NetworkServerManager.Singleton.Server.DisconnectClient(player2.Value.Id);
            //        //}
            //    }
            //}
        }

        return returnedAttempt;
    }

}
