using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public string IP;
    public GUIControl disconnectedPopup;
    public TMP_InputField TMPIPTIp;

    public void Start()
    {
        try { TMPIPTIp.text = PlayerPrefs.GetString("IPSelected"); }
        catch { }
    }
    public void onMainMenuPressed()
    {
        SceneManager.LoadScene("Assets/Scenes/MainMenu.unity", LoadSceneMode.Single);
    }

    public void onSingleplayerPressed()
    {
        SceneManager.LoadScene("Assets/Scenes/SinglePlayer.unity", LoadSceneMode.Single);
    }

    public void onMultiplayerSelectionPressed()
    {
        try
        {
            NetworkClientManager.Singleton.Client?.Disconnect();
        }
        catch { }
        SceneManager.LoadScene("Assets/Scenes/Multiplayer_Select.unity", LoadSceneMode.Single);
    }
    
    public void onAvatarSelectionPressed()
    {
        SceneManager.LoadScene("Assets/Scenes/AvatarMenu.unity", LoadSceneMode.Single);
    }

    public void onAboutPressed()
    {
        SceneManager.LoadScene("Assets/Scenes/About.unity", LoadSceneMode.Single);
    }

    public void onQuitPressed()
    {
        //fecha o jogo
        Application.Quit();
    }

    public void onServerIPChanged(string IP)
    {
        //this.IP = IP;
        PlayerPrefs.SetString("IPSelected", IP);
        //this.IP = IP.Replace(".", "");
    }

    public void onMultiplayerClientPressed()
    {
        PlayerPrefs.SetString("multiPlayerMode", "client");
        PlayerManager.multiPlayerMode = "client";
        SceneManager.LoadScene("Assets/Scenes/lobby.unity", LoadSceneMode.Single);
    }

    public void onMultiplayerServerPressed()
    {
        PlayerPrefs.SetString("multiPlayerMode", "server");
        PlayerManager.multiPlayerMode = "server";
        SceneManager.LoadScene("Assets/Scenes/lobby.unity", LoadSceneMode.Single);
    }

    public void onMatchStartClient()
    {
        FindObjectOfType<GUILobbyManager>().hideAll();
        PlayerManager.isInLobby = false;
        SceneManager.LoadScene("Assets/Scenes/Multi_Client.unity", LoadSceneMode.Additive);
    }

    public void onMatchStartServer()
    {
        FindObjectOfType<GUILobbyManager>().hideAll();
        PlayerManager.isInLobby = false;
        SceneManager.LoadScene("Assets/Scenes/Multi_Server.unity", LoadSceneMode.Additive);
    }

    public void onDisconnected()
    {
        disconnectedPopup.MenuToggle();
    }
}
