using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ServerManager : MonoBehaviour
{
    public InputField iP;
    public InputField port;

    public Button startHost;
    public Button startClient;

    private void Start() 
    {
        startHost.onClick.AddListener(() =>
        {
            //if(NetworkClientManager.Singleton.StartHost())
            //{
            //    Debug.Log("Servidor iniciado...");
            ChangeScene();
            //}
            //else Debug.Log("Error...");
        });

        startClient.onClick.AddListener(() =>
        {
            //if(NetworkClientManager.Singleton.StartClient())
            //    Debug.Log("Client iniciado...");
            //else Debug.Log("Error...");
        });
    }

    private static void ChangeScene()
    {
        SceneManager.LoadScene (sceneName:"game");
    }

    public void SingleplayerMode()
    {
        SceneManager.LoadScene (sceneName:"gamemode_1");
    }
}
