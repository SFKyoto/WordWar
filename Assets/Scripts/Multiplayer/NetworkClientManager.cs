using RiptideNetworking;
using RiptideNetworking.Utils;
using System;
using UnityEngine;

public enum ServerToClientId : ushort
{
    wordAnswer = 2,
    playerStats = 3,
    serverQuitGame = 4,
    youLost = 5,
    gameOver = 6,
}

public enum ClientToServerId : ushort
{
    playerDataMsg = 1,
    wordGuess = 2,
}

public class NetworkClientManager : MonoBehaviour
{
    private static NetworkClientManager _singleton;
    public static NetworkClientManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(NetworkClientManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    public Client Client { get; private set; }
    //public string Ip { get => ip; set => ip = value; }
    //public ushort Port { get => port; set => port = value; }
    public void SetSocket(string _ip, ushort _port)
    {
        ip = _ip;
        port = _port;
        Debug.Log("Socket set.");
    }

    [SerializeField] static string ip;
    [SerializeField] static ushort port;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        if (PlayerPrefs.GetString("multiPlayerMode") == "client")
        {
            ip = PlayerPrefs.GetString("IPSelected");
            Debug.Log("IP: " + ip);
            port = 1237;
            StartConn();

            Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.playerDataMsg);
            message.AddString(JsonUtility.ToJson(FindObjectOfType<AvatarManager>().GetPlayerData()));
            try
            {
                Client.Send(message);
            }
            catch(Exception e)
            {
                FindObjectOfType<GUILobbyManager>().FailedToConnect();
            }
        }
        else Destroy(this);
    }
    public void StartConn()
    {
        Debug.Log("Starting client in ip " + ip + ", port " + port.ToString());
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Client = new Client();
        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.ClientDisconnected += PlayerLeft;
        Client.Disconnected += DidDisconnect;
        Connect();
    }

    private void FixedUpdate()
    {
        Client.Tick();
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Disconnecting Client...");
        if (Client != null) Client.Disconnect();
    }

    public void Connect()
    {
        Debug.Log($"Connecting at {ip}:{port}...");
        Debug.Log(Client);
        Client.Connect($"{ip}:{port}");
    }

    private void DidConnect(object sender, EventArgs e)
    {
        Debug.Log("Player did connect");
        //UIManager.Singleton.SendName();
    }

    private void FailedToConnect(object sender, EventArgs e)
    {
        Debug.Log("Failed to connect");
        FindObjectOfType<GUILobbyManager>().FailedToConnect();
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        Debug.Log("Player left");
        //Destroy(Player.list[e.Id].gameObject);
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
        Debug.Log("Player did disconnect");
        FindObjectOfType<GUILobbyManager>().DidDisconnect();
    }
}