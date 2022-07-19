using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Gamesettings : MonoBehaviour
{
    public int maxVillager = 100;
    public int startResources = 1;
    public int maxPlayers = 2;
    private int p1Color= 1;
    private int p2Color= 2;
    private int p3Color= 3;
    private int p4Color= 4;
    public List<GameObject> playerUI;
    
    public Text villagers;

    public Text resources;

    public Text players;

    public Image teamImageP1;
    public Image teamImageP2;
    public Image teamImageP3;
    public Image teamImageP4;
    public Text teamNumberP1;
    public Text teamNumberP2;
    public Text teamNumberP3;
    public Text teamNumberP4;
    
    public bool isAllowedToSend = true;
    public void IncreaseVillagers()
    {
        if (maxVillager >= 200)
            return;
        maxVillager += 25;
        villagers.text = ""+maxVillager;
        ClientMessages.TransferLobbySettings(Client.myCurrentServer,maxPlayers,maxVillager,startResources);
    }

    public void DecreaseVillagers()
    {
        if (maxVillager <= 25)
            return;
        maxVillager -= 25;
        villagers.text = ""+maxVillager;
        ClientMessages.TransferLobbySettings(Client.myCurrentServer,maxPlayers,maxVillager,startResources);
    }

    public void IncreaseResources()
    {
        startResources++;
        if (startResources > 3)
            startResources = 1;
        SetResource(startResources);
        ClientMessages.TransferLobbySettings(Client.myCurrentServer,maxPlayers,maxVillager,startResources);
    }

    public void DecreaseResources()
    {
        startResources--;
        if (startResources < 1)
            startResources = 3;
        SetResource(startResources);
        ClientMessages.TransferLobbySettings(Client.myCurrentServer,maxPlayers,maxVillager,startResources);
    }

    public void SetResource(int number)
    {
        if (number == 1)
            resources.text = "Standard";
        if (number == 2)
            resources.text = "Plenty";
        if (number == 3)
            resources.text = "High";
        if(isAllowedToSend)
            ClientMessages.TransferLobbySettings(Client.myCurrentServer,maxPlayers,maxVillager,startResources);
        isAllowedToSend = true;
    }

    public void IncreasePlayers()
    {
        if (maxPlayers >= 4)
            return;
        maxPlayers += 1;
        if (maxPlayers == 3)
            playerUI[0].gameObject.SetActive(true);
        if (maxPlayers == 4)
            playerUI[1].gameObject.SetActive(true);
        players.text = "" +maxPlayers;
        if(isAllowedToSend)
            ClientMessages.TransferLobbySettings(Client.myCurrentServer,maxPlayers,maxVillager,startResources);
        isAllowedToSend = true;
    }

    public void DecreasePlayers()
    {
        if (maxPlayers <= 2)
            return;
        maxPlayers -= 1;
        if (maxPlayers == 3)
            playerUI[1].gameObject.SetActive(false);
        if (maxPlayers == 2)
            playerUI[0].gameObject.SetActive(false); 
        players.text = "" +maxPlayers;
        if(isAllowedToSend)
            ClientMessages.TransferLobbySettings(Client.myCurrentServer,maxPlayers,maxVillager,startResources);
        isAllowedToSend = true;
    }

    public void TeamColorPlayer1()
    {
        p1Color++;
        if (p1Color >= 5)
            p1Color = 1;
        SwitchColor(1,p1Color, teamImageP1);
        teamNumberP1.text = ""+p1Color;
    }

    public void TeamColorPlayer2()
    {
        p2Color++;
        if (p2Color >= 5)
            p2Color = 1;
        SwitchColor(2,p2Color, teamImageP2);
        teamNumberP2.text = ""+p2Color;
    }

    public void TeamColorPlayer3()
    {
        p3Color++;
        if (p3Color >= 5)
            
            p3Color = 1;
        SwitchColor(3,p3Color, teamImageP3);
        teamNumberP3.text = ""+p3Color;
    }

    public void TeamColorPlayer4()
    {
        p4Color++;
        if (p4Color >= 5)
            p4Color = 1;
        SwitchColor(4,p4Color, teamImageP4);
        teamNumberP4.text = ""+p4Color;
    }

    public void SwitchColor(int player,int colorNumber, Image teamcolor)
    {
        switch (colorNumber)
        {
            case 1:
                teamcolor.color = Color.cyan;
                break;
            case 2:
                teamcolor.color = Color.black;
                break;
            case 3:
                teamcolor.color = Color.gray;
                break;
            case 4:
                teamcolor.color = Color.yellow;
                break;
        }
        if(isAllowedToSend)
            ClientMessages.TransferTeamColor(Client.myCurrentServer, player);
        isAllowedToSend = true;
    }
}
