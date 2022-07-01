using System;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static Serverlist serverlist;
    public static LobbyManager lobbyManager;
    public static Tcp tcp;
    private static RTSCommunicator _communicator;
    private string serverIP = "";
    public static int clientID = 0;
    public static int otherID = 0;
    public static int myCurrentServer = 0;
    private static int dataBufferSize = 2048;
    public int serverPort;
    private static bool isConnected;
    private static SynchronizationContext _synchronizationContextField;
    private void Awake()
    {
        _synchronizationContextField = SynchronizationContext.Current;
        DontDestroyOnLoad(transform);
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
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
        _communicator = transform.GetComponent<RTSCommunicator>();
        LoadSettings();
        tcp = new Tcp();
        StartClient();
    }

    public void LoadSettings()
    {
        serverIP = "89.58.11.216"; //PlayerPrefs.GetString("ServerIP");
        serverPort = 11000; //Convert.ToInt32(PlayerPrefs.GetString("ServerPort"));
    }

    private static void StartClient()
    {
        isConnected = true;
        tcp.Connect();
    }

    public class Tcp
    {
        public TcpClient socket;
        private NetworkStream stream;
        private SimpleObject receiverObject;
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

            receiverObject = new SimpleObject();
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(SimpleObject data)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(data.ReturnBufferList().ToArray(), 0, data.ReturnBufferList().Count, null, null);
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
                HandleData(receiveBuffer, receiverObject);
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                Disconnect();
            }
        }

        public static void HandleData(byte[] _receiveBuffer, SimpleObject receiverObject)
        {
            receiverObject.SetBytes(_receiveBuffer);
            if (!ClientMessages.welcomePackageReceived)
            {
                ClientMessages.OnFirstConnect(receiverObject);
                ClientMessages.welcomePackageReceived = true;
            }
            else
            {
                string serverCall = receiverObject.ReadStringRange();
                int unit_id;
                Vector3 pos;
                Quaternion rot;
                switch (serverCall)
                {
                    case "start_match":
                        serverlist.GameStarted();
                        lobbyManager.InitGame();
                        break;

                    case "receive_serverlist":
                        serverlist.ClearBuffers();
                        lobbyManager.ClearServerlist();
                        int serverAmount = receiverObject.ReadIntRange();
                        for (int i = 1; i <= serverAmount; i++)
                        {
                            int owner_id = receiverObject.ReadIntRange();
                            string owner_name = receiverObject.ReadStringRange();
                            int secondplayer_id = receiverObject.ReadIntRange();
                            string secondplayer_name = receiverObject.ReadStringRange();
                            string server_name = receiverObject.ReadStringRange();
                            bool full = receiverObject.ReadBoolRange();
                            bool currentlyIngame = receiverObject.ReadBoolRange();

                            serverlist.CreateServer(owner_id, owner_name, server_name);
                            var serverEntry = serverlist.ServerlistDictionary[owner_id];
                            serverEntry.player2_id = secondplayer_id;
                            serverEntry.player2_name = secondplayer_name;
                            serverEntry.server_full = full;
                            serverEntry.currently_ingame = currentlyIngame;

                            if (!serverEntry.currently_ingame)
                                lobbyManager.CreateSingleServerlistEntry(owner_id);
                        }

                        break;

                    case "playerjoined":
                        int joiningplayer_id = receiverObject.ReadIntRange();
                        string player_name = receiverObject.ReadStringRange();

                        var serverjoined = serverlist.ServerlistDictionary[clientID];
                        serverjoined.player2_id = joiningplayer_id;
                        serverjoined.player2_name = player_name;
                        if (serverjoined.player1_id != 0 && serverjoined.player2_id != 0)
                        {
                            otherID = joiningplayer_id;
                            serverjoined.server_full = true;
                            lobbyManager.PlayerJoinedMatch(player_name);
                        }

                        break;

                    case "playerleft":
                        otherID = 0;
                        lobbyManager.PlayerLeftMatch();
                        break;

                    case "server_transfer":
                        int server_id = receiverObject.ReadIntRange();
                        var transferserver = serverlist.ServerlistDictionary[server_id];
                        transferserver.player1_id = clientID;
                        transferserver.player1_name = lobbyManager.localplayer_name;
                        transferserver.player2_id = 0;
                        transferserver.player2_name = "";
                        transferserver.server_full = false;
                        serverlist.ServerlistDictionary.Add(clientID, transferserver);
                        serverlist.ServerlistDictionary.Remove(server_id);
                        lobbyManager.TransferServer();
                        break;
                    case "unit_created":
                        //Scriptableobject auslesen
                        unit_id = receiverObject.ReadIntRange();
                        string prefab_name = receiverObject.ReadStringRange();
                        pos = receiverObject.ReadVector3Range();
                        rot = receiverObject.ReadQuaternionRange();
                        UnitData unit = new UnitData(unit_id, prefab_name, pos, rot, 100, 15, 15, 15);
                        _synchronizationContextField.Post(_ =>
                        {
                            unit.unit = Instantiate(Resources.Load(prefab_name+"_prefab"),pos,rot) as GameObject;
                        }, null);
                        serverlist.ServerlistDictionary[myCurrentServer].PlayerDictionary[otherID].UnitDictionary
                            .Add(unit_id, unit);
                        break;
                    case "update_unit_pos":
                        unit_id = receiverObject.ReadIntRange();
                        pos = receiverObject.ReadVector3Range();
                        rot = receiverObject.ReadQuaternionRange();
                        _synchronizationContextField.Post(_ =>
                        {
                            UnitData unit_pos = serverlist.ServerlistDictionary[myCurrentServer].PlayerDictionary[otherID]
                                .UnitDictionary[unit_id];
                            unit_pos.unit.transform.position = pos;
                            unit_pos.unit.transform.rotation = rot;
                        }, null);
                        break;
                }
            }

            receiverObject.ClearBuffers();
        }
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