using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIMultiplayerManager : MonoBehaviour
{
    public Text[] TXTPlayerNicks;
    public Text[] TXTPlayerScores;
    public AvatarManager[] Avatars;
    public GUIControl disconnectedPopup;
    public Slider SLDTempoRestante;
    public TextMeshProUGUI TXTCurrentRound;
    public Boolean isMPlayerModeServer;

    public GUIControl guiWinningScreenControl;
    public TextMeshProUGUI TXTWinningPlayerNick;
    public TextMeshProUGUI TXTWinningPlayerScore;
    public AvatarManager WinningPlayerAvatar;

    private void Start()
    {
        isMPlayerModeServer = PlayerManager.multiPlayerMode == "server";

        for (int i = 0; i < TXTPlayerNicks.Length; i++)
            TXTPlayerNicks[i].text = TXTPlayerScores[i].text = "";
    }

    /// <summary>
    /// Exibe popup de desconexão na tela.
    /// </summary>
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
        //Debug.Log(players.Length);
        //Debug.Log(playerList[0]);
        //Debug.Log(players[0]);

        for (int i = 0; i < TXTPlayerNicks.Length; i++)
        {
            TXTPlayerNicks[i].text = players.Length <= i ? "" : (players[i].username);
            TXTPlayerScores[i].text = players.Length <= i ? "" : (players[i].score + " pts");
            if (players.Length > i) Avatars[i].SetPlayerData(players[i]);
            else Avatars[i].ClearAvatar();
        }
    }

    /// <summary>
    /// Exibe o popup de jogador vencedor.
    /// </summary>
    public void ShowWinningPlayer(PlayerData playerData)
    {
        FindObjectOfType<WordManager>().BecomeObserver();
        FindObjectOfType<MultiPlayerServerGuessesManager>().timerStarted = false;
        guiWinningScreenControl.MenuToggle();
        TXTWinningPlayerNick.text = playerData.username;
        TXTWinningPlayerScore.text = playerData.score + " pts";
        WinningPlayerAvatar.SetPlayerData(playerData);
    }
}
