using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string IP;
    
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
        this.IP = IP;
        //this.IP = IP.Replace(".", "");
    }

    public void onMultiplayerClientPressed()
    {

        //this.IP = IP.Replace(".", "");
    }

    public void onMultiplayerServerPressed()
    {
        SceneManager.LoadScene("Assets/Scenes/lobby.unity", LoadSceneMode.Single);
        //NetworkServerManager.Singleton.Server.Start(1237, 3);
        //para tela de lobby
    }
}
