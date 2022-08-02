using UnityEngine;

public class ClientHandler
{
    public static void OnFirstConnect(Packet welcomeMessage)
    {
        string msg = welcomeMessage.ReadString();
        int myId = welcomeMessage.ReadInt();
        Client.clientID = myId;
        Client.lobbyManager.playerui_id.text = "Player ID: " +myId; 
        Client.lobbyManager.connectionText.text = "Connected";
        Client.lobbyManager.connectionStatus.color = Color.green;
        Debug.Log(msg);
        ClientMessages.UserIdReceived();
    }

    public static void StartMatch(Packet packet)
    {
        Client.serverlist.GameStarted();
        Client.lobbyManager.InitGame();
    }

    public static void ReceiveServerlist(Packet packet)
    {
        Client.serverlist.ClearBuffers();
        Client.lobbyManager.ClearServerlist();
        int serverAmount = packet.ReadInt();
        for (int i = 1; i <= serverAmount; i++)
        {
            int owner_id = packet.ReadInt();
            string owner_name = packet.ReadString();
            int secondplayer_id = packet.ReadInt();
            string secondplayer_name = packet.ReadString();
            string server_name = packet.ReadString();
            bool full = packet.ReadBool();
            bool currentlyIngame = packet.ReadBool();

            Client.serverlist.CreateServer(owner_id, owner_name, server_name);
            var serverEntry = Client.serverlist.ServerlistDictionary[owner_id];
            serverEntry.player2_id = secondplayer_id;
            serverEntry.player2_name = secondplayer_name;
            serverEntry.server_full = full;
            serverEntry.currently_ingame = currentlyIngame;

            if (!serverEntry.currently_ingame)
                Client.lobbyManager.CreateSingleServerlistEntry(owner_id);
        }
    }

    public static void PlayerJoined(Packet packet)
    {
        int joiningplayer_id = packet.ReadInt();
        string player_name = packet.ReadString();
        var serverjoined = Client.serverlist.ServerlistDictionary[Client.clientID];
        serverjoined.player2_id = joiningplayer_id;
        serverjoined.player2_name = player_name;
        if (serverjoined.player1_id != 0 && serverjoined.player2_id != 0)
        {
            Client.otherID = joiningplayer_id;
            serverjoined.server_full = true;
            Client.lobbyManager.PlayerJoinedMatch(player_name);
        }
    }

    public static void PlayerLeft(Packet packet)
    {
        Client.otherID = 0;
        Client.lobbyManager.PlayerLeftMatch();
    }

    public static void ServerTransfered(Packet packet)
    {
        int server_id = packet.ReadInt();
        var transferserver = Client.serverlist.ServerlistDictionary[server_id];
        transferserver.player1_id = Client.clientID;
        transferserver.player1_name = Client.lobbyManager.localplayer_name;
        transferserver.player2_id = 0;
        transferserver.player2_name = "";
        transferserver.server_full = false;
        Client.serverlist.ServerlistDictionary.Add(Client.clientID, transferserver);
        Client.serverlist.ServerlistDictionary.Remove(server_id);
        Client.lobbyManager.TransferServer();
    }

    public static void UnitCreated(Packet packet)
    {
        int unit_id = packet.ReadInt();
        unit_id = Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.otherID]
            .unitcounter;
        string prefab_name = packet.ReadString();
        Vector3 pos = packet.ReadVector3();
        Quaternion rot = packet.ReadQuaternion();
        UnitData unit = new UnitData(unit_id, prefab_name,pos,rot);
        unit.SpawnIngameUnit();
        unit.unit.transform.tag = "Player2";
        unit.unit.layer = 8; // 8 = enemy
        unit.unit.GetComponent<RTSView>().unit_id = unit_id;
        Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.otherID].UnitDictionary
            .Add(unit_id, unit);
        Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.otherID].UnitCounter();
    }

    public static void BuildingCreated(Packet packet)
    {
        int unit_id = packet.ReadInt();
        string prefab_name = packet.ReadString();
        Vector3 pos = packet.ReadVector3();
        Quaternion rot = packet.ReadQuaternion();
        BuildingData building = new BuildingData(unit_id, prefab_name,pos,rot);
        building.SpawnIngameBuilding();
        building.building.transform.tag = "Player2";
        building.building.layer = 8; // 8 = enemy
        Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.otherID].BuildingDictionary
            .Add(unit_id, building);
    }
    
    public static void UnitPosUpdated(Packet packet)
    {
        int unit_id = packet.ReadInt();
        Vector3 pos = packet.ReadVector3();
        UnitData unit_pos = Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.otherID]
            .UnitDictionary[unit_id];
        unit_pos.currentlyMovingToPos = pos;
        unit_pos.MoveTo();
    }
    public static void SettingsUpdated(Packet packet)
    {
        Gamesettings gamesettings = Client.lobbyManager.settings;
        int players = packet.ReadInt();
        int villagers = packet.ReadInt();
        int startResources = packet.ReadInt();
        gamesettings.isAllowedToSend = false;
        gamesettings.SetResource(startResources);
        gamesettings.villagers.text = "" + villagers;
        gamesettings.startResources = startResources;
        gamesettings.maxVillager = villagers;
        
        Client.serverlist.ServerlistDictionary[Client.myCurrentServer].max_villagers = villagers;
        Client.serverlist.ServerlistDictionary[Client.myCurrentServer].max_players = players;
        Client.serverlist.ServerlistDictionary[Client.myCurrentServer].start_resources = startResources;
        
        if (gamesettings.maxPlayers < players)
        {
            gamesettings.isAllowedToSend = false;
            gamesettings.IncreasePlayers();
            return;
        }
        if (gamesettings.maxPlayers > players)
        {
            gamesettings.isAllowedToSend = false;
            gamesettings.DecreasePlayers();
        }
    }
    public static void TeamSettingUpdated(Packet packet)
    {
        Client.lobbyManager.settings.isAllowedToSend = false;
        switch (packet.ReadInt())
        {
            case 1: Client.lobbyManager.settings.TeamColorPlayer1();
                break;
            case 2: Client.lobbyManager.settings.TeamColorPlayer2();
                break;
            case 3: Client.lobbyManager.settings.TeamColorPlayer3();
                break;
            case 4: Client.lobbyManager.settings.TeamColorPlayer4();
                break;
        }
    }
    
    public static void BuildBuilding(Packet packet)
    {
        int player = packet.ReadInt();
        int building_id = packet.ReadInt();
        int multiplier = packet.ReadInt();
        bool initialized = packet.ReadBool() ;
        bool finished =  packet.ReadBool();
        GameObject currentbuilding = null;
        BuildingSelected select = null;
        if (initialized)
        {
            currentbuilding = Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[player]
                .BuildingDictionary[building_id].building;
            select = currentbuilding.GetComponent<BuildingSelected>();
            select.finishedBuilding = select.InitBuildingMultiplayer();
            return;
        }

        if (finished)
            return;
        
        currentbuilding = Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[player]
            .BuildingDictionary[building_id].building;
        select = currentbuilding.GetComponent<BuildingSelected>();
        select.buildinghp += 1 *multiplier;
        select.progress = select.transform.Find("Progress").GetComponent<TextMesh>();
        select.progress.text = select.buildinghp + "|" + select.buildingData.buildingtime;
        select.finishedBuilding.transform.position += new Vector3(0,20f/select.buildingData.buildingtime*multiplier,0);
        if (select.buildinghp >= select.buildingData.buildingtime)
        {
            currentbuilding.SetActive(false);
            Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[player]
                .BuildingDictionary[building_id].building = select.finishedBuilding;
        }
    }

    public static void ReadyCheck(Packet packet)
    {
        int player = packet.ReadInt();
        bool ready = packet.ReadBool();
        Client.lobbyManager.settings.ReadyCheck(player,ready);
    }

    public static void DestroyResource(Packet packet)
    {
        string resource_id = packet.ReadString();
        GatheringHandler.RemoveResource(resource_id);
    }

    public static void SpawnProjectile(Packet packet)
    {
        Debug.Log("called");
        int shooterid = packet.ReadInt();
        int targetid = packet.ReadInt();

        GameObject attacker = Client.serverlist.ServerlistDictionary[Client.myCurrentServer]
            .PlayerDictionary[Client.otherID].UnitDictionary[shooterid].unit;
        GameObject target = Client.serverlist.ServerlistDictionary[Client.myCurrentServer]
            .PlayerDictionary[Client.clientID].UnitDictionary[targetid].unit;

        if (Client.serverlist.ServerlistDictionary[Client.myCurrentServer]
                .PlayerDictionary[Client.clientID].UnitDictionary[targetid].current_hp <= 0)
            return;
        GameObject arrow = ObjectSpawner.SpawnObject("Arrow", attacker.transform.position+attacker.transform.forward, new Quaternion());
        arrow.GetComponent<Arrow>().fromEnemy = true;
        arrow.GetComponent<Arrow>().ShootArrowOn(target,Client.serverlist.ServerlistDictionary[Client.myCurrentServer]
            .PlayerDictionary[Client.otherID].UnitDictionary[shooterid].damage);
    }
}
