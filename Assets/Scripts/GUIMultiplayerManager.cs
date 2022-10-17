using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GUIMultiplayerManager : MonoBehaviour
{
    public Text[] TXTPlayerNicks;
    public Text[] TXTPlayerScores;
    public GUIControl disconnectedPopup;
    public Boolean isMPlayerModeServer;

    private void Start()
    {
        isMPlayerModeServer = PlayerPrefs.GetString("multiPlayerMode") == "server";

        for (int i = 0; i < TXTPlayerNicks.Length; i++)
            TXTPlayerNicks[i].text = TXTPlayerScores[i].text = "";
    }

    public void DidDisconnect()
    {
        disconnectedPopup.MenuToggle();
    }

    /// <summary>
    /// Atualiza dados dos jogadores na tela.
    /// </summary>
    public void UpdatePlayers(Dictionary<ushort, PlayerData> playerList)
    {
        PlayerData[] players = playerList.Values.ToArray();
        Debug.Log(players.Length);
        Debug.Log(playerList[0]);
        Debug.Log(players[0]);

        for (int i = 0; i < TXTPlayerNicks.Length; i++)
        {
            TXTPlayerNicks[i].text = players.Length <= i ? "" : (players[i].username);
            TXTPlayerScores[i].text = players.Length <= i ? "" : (players[i].score + " pts");
        }
    }

}
