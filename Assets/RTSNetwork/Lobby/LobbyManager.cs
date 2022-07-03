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
        startButton.enabled = true;
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
            ClientMessages.StartGamelobby(server.player2_id);
        }
    }

    public void InitGame()
    {
        SceneManager.LoadScene("Playground");
    }

    public void AddServertoServerList()
    {
        Client.serverlist.CreateServer(Client.clientID, localplayer_name, roomNameEntered);
        ClientMessages.AddGamelobby(localplayer_name, roomNameEntered);
        isHostingLobby = true;
        startButton.enabled = true;
    }

    public void JoinMatch(int lobby_id)
    {
        ClientMessages.JoinLobby(lobby_id, localplayer_name);
        Client.myCurrentServer = lobby_id;
        Client.otherID = lobby_id;
        player1lobbyName.text = Client.serverlist.ServerlistDictionary[lobby_id].player1_name;
        player2lobbyName.text = localplayer_name;
        InitNextUIElement(LobbyRoom.gameObject);
        startButton.enabled = false;
    }

    public void PlayerJoinedMatch(string playerName)
    {
        player2lobbyName.text = playerName;
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
