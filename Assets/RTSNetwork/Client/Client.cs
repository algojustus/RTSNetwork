using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static Serverlist serverlist;
    public static LobbyManager lobbyManager;
    public static Tcp tcp;
    private string serverIP = "";
    public static int clientID = 0;
    public static int otherID = 0;
    public static int myCurrentServer = 0;
    public static string myGameColor;
    private static int dataBufferSize = 2048;
    public int serverPort;
    private static bool isConnected;

    private delegate void PacketHandler(Packet packet);

    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, true, 60);
        DontDestroyOnLoad(transform);
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    private void Start()
    {
        GameObject find = GameObject.Find("LobbyManager");
        lobbyManager = find.GetComponent<LobbyManager>();
        LoadSettings();
        tcp = new Tcp();
        StartClient();
    }

    public void LoadSettings()
    {
        serverIP = "89.58.11.216"; //PlayerPrefs.GetString("ServerIP");
        serverPort = 11000; //Convert.ToInt32(PlayerPrefs.GetString("ServerPort"));
    }

    private void StartClient()
    {
        isConnected = true;
        InitializeClientData();
        tcp.Connect();
    }

    public class Tcp
    {
        public TcpClient socket;
        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public void Connect()
        {
            serverlist = new Serverlist();
            socket = new TcpClient()
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };
            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.serverIP, instance.serverPort, ConnectCallback, socket);
        }

        public void Disconnect()
        {
            Client.Disconnect();
            stream = null;
            receiveBuffer = null;
            socket = null;
        }

        private void ConnectCallback(IAsyncResult result)
        {
            socket.EndConnect(result);
            if (!socket.Connected)
                return;
            stream = socket.GetStream();

            receivedData = new Packet();
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int messageByteLength = stream.EndRead(result);
                if (messageByteLength <= 0)
                {
                    Client.Disconnect();
                    return;
                }

                byte[] data = new byte[messageByteLength];
                Array.Copy(receiveBuffer, data, messageByteLength);

                receivedData.Reset(HandleData(data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                Disconnect();
            }
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }
            }

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        packetHandlers[_packetId](_packet);
                    }
                });
            
                _packetLength = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (_packetLength <= 1)
            {
                return true;
            }

            return false;
        }
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int) ServerPackets.welcome, ClientHandler.OnFirstConnect},
            {(int) ServerPackets.serverTransfered, ClientHandler.ServerTransfered},
            {(int) ServerPackets.gameStarted, ClientHandler.StartMatch},
            {(int) ServerPackets.playerLeft, ClientHandler.PlayerLeft},
            {(int) ServerPackets.playerJoined, ClientHandler.PlayerJoined},
            {(int) ServerPackets.serverlistRequested, ClientHandler.ReceiveServerlist},
            {(int) ServerPackets.spawnPlayer, ClientHandler.UnitCreated},
            {(int) ServerPackets.playerPosition, ClientHandler.UnitPosUpdated},
            {(int) ServerPackets.spawnBuilding, ClientHandler.BuildingCreated}
        };
    }

    private static void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();
        }
    }
}
