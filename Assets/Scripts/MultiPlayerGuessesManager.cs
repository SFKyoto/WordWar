using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class MultiPlayerGuessesManager : GameGuessesManager
{
    //public NetworkClientManager networkClientManager;
    private void Awake()
    {
        Debug.Log("multiplayer client started.");
    }

    public override void GenerateNewAnswer() { }

    public override string GetCheckedAttempt(string currentGuess)
    {
        Debug.Log($"Multiplayer message: {currentGuess}");
        //Debug.Log(networkClientManager);
        //if (networkClientManager == null) SetConn();
        //networkClientManager.SendMessage(currentGuess);

        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.wordGuess);
        message.AddString(currentGuess);
        NetworkClientManager.Singleton.Client.Send(message);

        return "";
    }

}
