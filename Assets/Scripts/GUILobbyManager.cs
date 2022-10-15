using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUILobbyManager : MonoBehaviour
{
    public TextMeshProUGUI TXTIpAddress;
    public TextMeshProUGUI[] TXTPlayerData = new TextMeshProUGUI[4];
    public Boolean isMPlayerModeServer;
    public Button BTNStartMatch;

    private void Start()
    {
        if (isMPlayerModeServer)
        {
            TXTIpAddress.text = new WebClient().DownloadString("https://api.ipify.org/");
        }
        else
        {
            TXTIpAddress.gameObject.SetActive(false);
        }
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

        for (int i = 0; i < TXTPlayerData.Length; i++)
        {
            TXTPlayerData[i].text = players.Length <= i ? "" : (players[i].username);
        }
        BTNStartMatch.gameObject.SetActive(isMPlayerModeServer && players.Length > 1);
        //BTNStartMatch.enabled = players.Length > 1;
    }
}
