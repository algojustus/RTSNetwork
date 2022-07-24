﻿using System.Collections.Generic;
using UnityEngine;

public class Serverlist
{
    public Dictionary<int, Serverlist> ServerlistDictionary;
    public Dictionary<int, PlayerData> PlayerDictionary;
    
    public int player1_id;
    public int player2_id;
    public int player3_id;
    public int player4_id;
    public bool player1_readycheck;
    public bool player2_readycheck;
    public bool player3_readycheck;
    public bool player4_readycheck;
    public string server_name = "";
    public string player1_name = "";
    public string player2_name = "";
    public string player3_name = "";
    public string player4_name = "";
    public bool currently_ingame;
    public bool server_full;
    public int max_villagers;
    public int start_resources;
    public int max_players;
    public int start_villagers = 5;
    

    public Serverlist(int creator_id, string creator_name, string _server_name)
    {
        server_full = false;
        player1_id = creator_id;
        server_name = _server_name;
        player1_name = creator_name;
    }

    public Serverlist()
    {
        ServerlistDictionary = new Dictionary<int, Serverlist>();
        PlayerDictionary = new Dictionary<int, PlayerData>();
    }

    public Dictionary<int, Serverlist> ReturnServerList()
    {
        return ServerlistDictionary;
    }

    public void CreateServer(int creator_id, string creator_name, string server_name)
    {
        Serverlist server = new Serverlist(creator_id, creator_name, server_name);

        if (!ServerlistDictionary.ContainsKey(creator_id))
        {
            ServerlistDictionary.Add(creator_id, server);
            return;
        }
        ServerlistDictionary.Remove(creator_id);
        ServerlistDictionary.Add(creator_id, server);
    }

    public void CloseServer(int creator_id)
    {
        if (ServerlistDictionary.ContainsKey(creator_id))
        {
            ServerlistDictionary.Remove(creator_id);
        }
    }

    public void PrintServerList()
    {
        foreach (var kvp in ServerlistDictionary)
        {
            Debug.Log("Server_ID = "+kvp.Key);
        }
    }

    public void GameStarted()
    {
        var server = ServerlistDictionary[Client.myCurrentServer];
        currently_ingame = true;
        server.PlayerDictionary = new Dictionary<int, PlayerData>();
        server.PlayerDictionary.Add(1, new PlayerData(server.player1_id,server.player1_name,"_blau"));
        server.PlayerDictionary.Add(2, new PlayerData(server.player2_id,server.player2_name,"_rot"));
    }

    public void ClearBuffers()
    {
        ServerlistDictionary.Clear();
    }
}