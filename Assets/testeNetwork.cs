using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class testeNetwork : MonoBehaviour
{

    public NetworkManager networkManager;

    public void StartHost()
    {
        networkManager.StartHost();
    }

    public void StartClient()
    {
        networkManager.StartClient();
    }
}
