using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void onMultiplayerPressed()
    {
        //Vai Pro lobby
    }

    public void onSingleplayerPressed()
    {
        //vai pro modo Single
        SceneManager.LoadScene("Assets/Scenes/SinglePlayer.unity", LoadSceneMode.Single);
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
}
