using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class MultiPlayerGuessesManager : GameGuessesManager
{
    public void Start()
    {
        if (timeBetweenGuessedWords <= 0)
            timeBetweenGuessedWords = 40.0f;
    }
    public void Update()
    {
        if (timerStarted)
        {
            timeLeftBetweenWords -= Time.deltaTime;
            if (timeLeftBetweenWords <= 0)
            {
                timeLeftBetweenWords = timeBetweenGuessedWords;
            }
            FindObjectOfType<GUIMultiplayerManager>().SLDTempoRestante.value = timeLeftBetweenWords / timeBetweenGuessedWords;
        }
    }

    //public NetworkClientManager networkClientManager;
    private void Awake()
    {
        Debug.Log("multiplayer client started.");
    }

    public override void GenerateNewAnswer() { }

    public override string GetCheckedAttempt(string currentGuess)
    {
        Debug.Log($"Multiplayer message: {(ushort)ClientToServerId.wordGuess} - {currentGuess}");

        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.wordGuess);
        message.AddString(currentGuess);
        //FindObjectOfType<NetworkClientManager>().Client.Send(message);
        NetworkClientManager.Singleton.Client.Send(message);

        return "";
    }

}
