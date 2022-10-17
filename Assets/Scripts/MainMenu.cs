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
        SceneManager.LoadScene("Assets/Scenes/Multiplayer_Select.unity", LoadSceneMode.Single);
    }
    
    public void onAvatarSelectionPressed()
    {
        SceneManager.LoadScene("Assets/Scenes/AvatarMenu.unity", LoadSceneMode.Single);
    }

    public void onAboutPressed()
    {
        //vai para os creditos
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
        SceneManager.LoadScene("Assets/Scenes/lobby.unity", LoadSceneMode.Single);
    }

    public void onMultiplayerServerPressed()
    {
        PlayerPrefs.SetString("multiPlayerMode", "server");
        SceneManager.LoadScene("Assets/Scenes/lobby.unity", LoadSceneMode.Single);
    }

    public void onMatchStartClient()
    {
        SceneManager.LoadScene("Assets/Scenes/Multi_Client.unity", LoadSceneMode.Single);
    }

    public void onMatchStartServer()
    {
        SceneManager.LoadScene("Assets/Scenes/Multi_Server.unity", LoadSceneMode.Single);
    }

    public void onDisconnected()
    {
        disconnectedPopup.MenuToggle();
    }
}
