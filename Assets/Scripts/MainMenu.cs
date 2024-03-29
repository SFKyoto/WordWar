using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string IP;
    public GUIControl disconnectedPopup;
    public TMP_InputField TMPIPTIp;
    public GUIControl guiWinningPlayer;
    public TextMeshProUGUI TXTInvalidCode;

    public void Start()
    {
        try { TMPIPTIp.text = PlayerPrefs.GetString("IPSelected"); }
        catch { }
        if (PlayerManager.isThereWinningPlayer)
        {
            guiWinningPlayer.showWinningPlayer();
            PlayerManager.isThereWinningPlayer = false;
        }
    }
    public static void onMainMenuPressed()
    {
        SceneManager.LoadScene("Assets/Scenes/MainMenu.unity", LoadSceneMode.Single);
    }

    public void onSingleplayerPressed()
    {
        SceneManager.LoadScene("Assets/Scenes/SinglePlayer.unity", LoadSceneMode.Single);
    }

    public void onMultiplayerSelectionPressed()
    {
        Debug.Log("menu:");
        Debug.Log(NetworkClientManager.Singleton);
        NetworkClientManager.Singleton?.Client?.Disconnect();
        NetworkServerManager.Singleton?.Server?.Stop();
        PlayerManager.playerList.Clear();
        SceneManager.LoadScene("Assets/Scenes/Multiplayer_Select.unity", LoadSceneMode.Single);
    }
    
    public void onAvatarSelectionPressed()
    {
        SceneManager.LoadScene("Assets/Scenes/AvatarMenu.unity", LoadSceneMode.Single);
    }

    public static void onAboutPressed()
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
        PlayerPrefs.SetString("IPSelected", IP.ToUpper());
        //this.IP = IP.Replace(".", "");
    }

    public void onMultiplayerClientPressed()
    {
        if(PlayerPrefs.GetString("IPSelected") != null && PlayerPrefs.GetString("IPSelected") != "" && PlayerPrefs.GetString("IPSelected").Length == 8)
        {
            PlayerManager.isInLobby = true;
            PlayerManager.multiPlayerMode = "client";
            SceneManager.LoadScene("Assets/Scenes/lobby.unity", LoadSceneMode.Single);
        }
        else
        {
            TXTInvalidCode.gameObject.SetActive(true);
        }
    }

    public void onMultiplayerServerPressed()
    {
        PlayerManager.isInLobby = true;
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
