﻿using UnityEngine;

public class ClientHandler
{
    public static void OnFirstConnect(Packet welcomeMessage)
    {
        string msg = welcomeMessage.ReadString();
        int myId = welcomeMessage.ReadInt();
        Client.clientID = myId;
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
        //Scriptableobject auslesen -> siehe bei buildings
        int unit_id = packet.ReadInt();
        string prefab_name = packet.ReadString();
        Vector3 pos = packet.ReadVector3();
        Quaternion rot = packet.ReadQuaternion();
        UnitData unit = new UnitData(unit_id, prefab_name,pos,rot);
        unit.SpawnIngameUnit();
        unit.unit.transform.tag = "Player2";
        Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.otherID].UnitDictionary
            .Add(unit_id, unit);
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
}
