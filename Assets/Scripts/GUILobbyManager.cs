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
        isMPlayerModeServer = PlayerPrefs.GetString("multiPlayerMode") == "server";
        if (isMPlayerModeServer)
        {
            TXTIpAddress.text = "Código da sala: " + new WebClient().DownloadString("https://api.ipify.org/");
        }
        else
        {
            TXTIpAddress.text = "Conectando...";
            foreach (var txtPlayerData in TXTPlayerData)
                txtPlayerData.text = "";
            //TXTIpAddress.gameObject.SetActive(false);
        }
        BTNStartMatch.gameObject.SetActive(isMPlayerModeServer);
    }

    public void DidDisconnect()
    {
        TXTIpAddress.text = "Desconectado do servidor.";
    }
    public void FailedToConnect()
    {
        TXTIpAddress.text = "Erro ao se conectar com o servidor.";
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
    }

}
