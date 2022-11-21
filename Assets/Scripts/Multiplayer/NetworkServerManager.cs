using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;

//public enum ServerToClientId : ushort
//{
//    playerSpawned = 1,
//}

//public enum ClientToServerId : ushort
//{
//    name = 1,
//}

public class NetworkServerManager : MonoBehaviour
{
    private static NetworkServerManager _singleton;
    public static NetworkServerManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(NetworkServerManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    public Server Server { get; private set; }

    [SerializeField] private ushort port;
    [SerializeField] private ushort maxClientCount;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        if (PlayerManager.multiPlayerMode == "server")
        {
            Debug.Log("Starting server...");
            Application.targetFrameRate = 60;

            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

            Server = new Server();
            Server.Start(port, maxClientCount);
            Server.ClientConnected += PlayerConnected;
            Server.ClientDisconnected += PlayerLeft;
        }
        else
        {
            Debug.Log("Está como cliente...");
            Destroy(this);
        }
    }

    private void FixedUpdate()
    {
        Server.Tick();
    }

    private void OnApplicationQuit()
    {
        if (Server != null)
        {
            Debug.Log("Quitting server...");
            Server.Stop();
        }
    }

    private void PlayerConnected(object sender, ServerClientConnectedEventArgs e)
    {
        Debug.Log("A player jooined");
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        Debug.Log("PlayerLeft: " + e.Id.ToString());
        FindObjectOfType<PlayerManager>().RemovePlayerFromList(e.Id);
        FindObjectOfType<PlayerManager>().CheckRemainingPlayers();
        //Destroy(PlayerManager.playerList[e.Id].gameObject); //vai dar erro! vc não está destruindo nada ainda
    }
}