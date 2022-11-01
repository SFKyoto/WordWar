using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using RiptideNetworking;
using System;
using Newtonsoft.Json;
using System.Linq;

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
        Debug.Log("multiplayer server manager started.");
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
        timerStarted = true;
        if (timeBetweenGuessedWords <= 0)
            timeBetweenGuessedWords = 300.0f;

        NetworkServerManager.Singleton.Server.SendToAll(Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.gameStart));
    }

    void Update()
    {
        if (timerStarted)
        {
            timeLeftBetweenWords -= Time.deltaTime;
            if (timeLeftBetweenWords <= 0)
            {
                barrierProgress++; //n necessario agora 

                var activePlayers = PlayerManager.playerList.Where(player => player.Value.active);
                var playerToDisconnect = activePlayers
                    .OrderBy(player => player.Value.score)
                    .ThenByDescending(player => player.Value.palavraAtual)
                    .ThenByDescending(player => player.Value.qtdTentativas)
                    .First();
                Debug.Log($"Should disconnect user {playerToDisconnect.Key}");

                if(playerToDisconnect.Value.id != 0)
                {
                    Message playerLostMessage = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.youLost);
                    NetworkServerManager.Singleton.Server.Send(playerLostMessage, playerToDisconnect.Value.id);
                }
                else
                {
                    FindObjectOfType<WordManager>().BecomeObserver();
                }
                FindObjectOfType<PlayerManager>().RemovePlayerFromList(playerToDisconnect.Value.id);

                
                //atualmente não há como dar empate
                if(activePlayers.Count() == 1)
                {
                    Message playerLostMessage = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.gameOver);
                    NetworkServerManager.Singleton.Server.SendToAll(playerLostMessage);
                    NetworkServerManager.Singleton.Server.Stop();
                    //para tela de vitória
                }

                timeLeftBetweenWords = timeBetweenGuessedWords;
            }

            Message playerStatsMsg = Message.Create(MessageSendMode.unreliable, (ushort)ServerToClientId.playerStats);
            playerStatsMsg.AddString(JsonConvert.SerializeObject(PlayerManager.playerList));
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

        if (PlayerManager.playerList[0].palavraAtual >= qtdPalavrasJogo)
        {
            Debug.Log($"GAME OVER - Player 0 won!");
            Message gameOverMessage = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.gameOver);
            gameOverMessage.AddUShort(0);
            NetworkServerManager.Singleton.Server.SendToAll(gameOverMessage);
            NetworkServerManager.Singleton.Server.Stop();
            //go to result screen
        }

        return returnedAttempt;
    }

}
