using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private InputField nameInputField;
    private Text nameInputText;
    public String localplayer_name = "";

    [SerializeField] private InputField roomNameInputField;
    private Text roomNameInputText;
    private String roomNameEntered = "";
    [SerializeField] private Text player1lobbyName;
    [SerializeField] private Text player2lobbyName;
    [SerializeField] private Canvas LobbyCreated;
    [SerializeField] private Canvas Lobby;
    [SerializeField] private Canvas ServerList;
    [SerializeField] private Canvas LobbyRoom;
    [SerializeField] private GameObject serverPrefab;
    [SerializeField] private GameObject serverGrid;
    [SerializeField] private Button startButton;
    private GameObject serverUIElement;
    private List<GameObject> serverElementList;
    private bool isHostingLobby;
    public static LobbyManager instance;
    public Gamesettings settings;
    public Text connectionText;
    public Text playerui_id;
    public Image connectionStatus;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        nameInputText = nameInputField.placeholder.GetComponent<Text>();
        roomNameInputText = nameInputField.placeholder.GetComponent<Text>();
        serverElementList = new List<GameObject>();
    }

    public void OnLobbyCreated()
    {
        localplayer_name = nameInputField.text;
        if (localplayer_name.Equals(""))
            NoNameEntered(nameInputText);
        else
        {
            InitNextUIElement(LobbyCreated.gameObject);
        }
    }

    public void OnLobbyStarted()
    {
        roomNameEntered = roomNameInputField.text;
        if (roomNameEntered.Equals(""))
            NoNameEntered(roomNameInputText);
        else
        {
            InitNextUIElement(LobbyRoom.gameObject);
            player1lobbyName.text = localplayer_name;
        }
    }

    public void OnServerListInitialized()
    {
        localplayer_name = nameInputField.text;
        if (localplayer_name.Equals(""))
            NoNameEntered(nameInputText);
        else
        {
            InitNextUIElement(ServerList.gameObject);
            ClientMessages.ReceiveServerlist();
        }
    }

    public void ReturnToStart()
    {
        InitNextUIElement(Lobby.gameObject);
    }

    public void TransferServer()
    {
        isHostingLobby = true;
        player1lobbyName.text = Client.serverlist.ServerlistDictionary[Client.clientID].player1_name;
        player2lobbyName.text = Client.serverlist.ServerlistDictionary[Client.clientID].player2_name;
        startButton.interactable = false;
    }

    public void RemoveServerOutOfServerList()
    {
        if (!isHostingLobby)
            return;
        ClientMessages.RemoveGamelobby();
        isHostingLobby = false;
    }

    public void GameStarted()
    {
        if (!isHostingLobby)
            return;
        var server = Client.serverlist.ServerlistDictionary[Client.clientID];
        if (server.player2_id != 0)
        {
            Client.myCurrentServer = Client.clientID;
            Client.myGameColor = "_blau";
            ClientMessages.StartGamelobby(server.player2_id);
        }
    }

    public void SetTestColor()
    {
        Client.myGameColor = "_blue";
    }

    public void InitGame()
    {
        SceneManager.LoadScene("Playground");
    }

    public void AddServertoServerList()
    {
        Client.myCurrentServer = Client.clientID;
        Client.serverlist.CreateServer(Client.clientID, localplayer_name, roomNameEntered);
        ClientMessages.AddGamelobby(localplayer_name, roomNameEntered);
        isHostingLobby = true;
        startButton.interactable = false;
    }

    public void JoinMatch(int lobby_id)
    {
        ClientMessages.JoinLobby(lobby_id, localplayer_name);
        Client.myCurrentServer = lobby_id;
        Client.otherID = lobby_id;
        Client.myGameColor = "_rot";
        Client.serverlist.ServerlistDictionary[lobby_id].player2_id =
            Client.clientID; //provisorisch und muss später über dynamische player ausgabe gemacht werden
        player1lobbyName.text = Client.serverlist.ServerlistDictionary[lobby_id].player1_name;
        player2lobbyName.text = localplayer_name;
        InitNextUIElement(LobbyRoom.gameObject);
        startButton.interactable = false;
    }

    public void PlayerJoinedMatch(string playerName)
    {
        player2lobbyName.text = playerName;
    }

    public void CheckifAllReady()
    {
        bool player1, player2, player3, player4;
        player1 = Client.serverlist.ServerlistDictionary[Client.myCurrentServer].player1_id > 0;
        player2 = Client.serverlist.ServerlistDictionary[Client.myCurrentServer].player2_id > 0;
        player3 = Client.serverlist.ServerlistDictionary[Client.myCurrentServer].player3_id > 0;
        player4 = Client.serverlist.ServerlistDictionary[Client.myCurrentServer].player4_id > 0;
        CheckForPlayers(player1, player2, player3, player4);
    }

    public void CheckForPlayers(bool p1, bool p2, bool p3, bool p4)
    {
        bool p1ready, p2ready, p3ready, p4ready;
        p1ready = !p1 || Client.serverlist.ServerlistDictionary[Client.myCurrentServer].player1_readycheck;
        p2ready = !p2 || Client.serverlist.ServerlistDictionary[Client.myCurrentServer].player2_readycheck;
        p3ready = !p3 || Client.serverlist.ServerlistDictionary[Client.myCurrentServer].player3_readycheck;
        p4ready = !p4 || Client.serverlist.ServerlistDictionary[Client.myCurrentServer].player4_readycheck;

        if (Client.myCurrentServer != Client.clientID)
            return;
        if (p1ready && p2ready && p3ready && p4ready)
            startButton.interactable = true;
        else
            startButton.interactable = false;
    }

    public void PlayerLeftMatch()
    {
        var server = Client.serverlist.ServerlistDictionary[Client.clientID];
        server.player2_id = default;
        server.player2_name = "";
        player2lobbyName.text = "";
    }

    public void LeaveMatch()
    {
        if (!isHostingLobby)
        {
            ClientMessages.LeaveLobby(Client.myCurrentServer);
            Client.myCurrentServer = 0;
            Client.otherID = 0;
            player1lobbyName.text = " - ";
            player2lobbyName.text = " - ";
            ReturnToStart();
        }
    }

    public void CreateServerlistEntry()
    {
        ClearServerlist();
        var serverlist = Client.serverlist.ReturnServerList();
        if (serverlist.Count == 0)
            return;

        foreach (var server in serverlist)
        {
            if (server.Value.currently_ingame)
                return;

            serverUIElement = Instantiate(serverPrefab, serverGrid.transform, false);
            serverUIElement.transform.GetChild(0).GetComponent<Text>().text = server.Value.server_name;
            serverUIElement.GetComponent<Button>().onClick.AddListener(
                delegate
                {
                    JoinMatch(server.Value.player1_id);
                });
            serverUIElement.transform.GetChild(1).GetComponent<Text>().text = "Players 1|2";
            if (server.Value.server_full)
            {
                serverUIElement.transform.GetChild(1).GetComponent<Text>().text = "Players 2|2";
                serverUIElement.GetComponent<Button>().interactable = false;
            }

            serverElementList.Add(serverUIElement);
        }
    }

    public void CreateSingleServerlistEntry(int server_id)
    {
        var server = Client.serverlist.ServerlistDictionary[server_id];
        serverUIElement = Instantiate(serverPrefab, serverGrid.transform, false);
        serverUIElement.transform.GetChild(0).GetComponent<Text>().text = server.server_name;
        serverUIElement.GetComponent<Button>().onClick.AddListener(
            delegate
            {
                JoinMatch(server.player1_id);
            });
        serverUIElement.transform.GetChild(1).GetComponent<Text>().text = "Players 1|2";
        if (server.server_full)
        {
            serverUIElement.transform.GetChild(1).GetComponent<Text>().text = "Players 2|2";
            serverUIElement.GetComponent<Button>().interactable = false;
        }

        serverElementList.Add(serverUIElement);
    }

    public void ClearServerlist()
    {
        foreach (var server in serverElementList)
        {
            Destroy(server);
        }

        serverElementList.Clear();
    }

    private void NoNameEntered(Text errorMessage)
    {
        errorMessage.text = "please enter name";
        errorMessage.color = Color.red;
    }

    private void InitNextUIElement(GameObject nextRoom)
    {
        Lobby.gameObject.SetActive(false);
        LobbyCreated.gameObject.SetActive(false);
        ServerList.gameObject.SetActive(false);
        LobbyRoom.gameObject.SetActive(false);
        nextRoom.SetActive(true);
    }
}
