using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class Gamesettings : MonoBehaviour
{
    private int maxVillager = 100;
    private int startResources = 1;
    private int maxPlayers = 2;
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
    public void IncreaseVillagers()
    {
        if (maxVillager >= 200)
            return;
        maxVillager += 25;
        villagers.text = ""+maxVillager;
    }

    public void DecreaseVillagers()
    {
        if (maxVillager <= 25)
            return;
        maxVillager -= 25;
        villagers.text = ""+maxVillager;
    }

    public void IncreaseResources()
    {
        startResources++;
        if (startResources > 3)
            startResources = 1;
        SetResource(startResources);
    }

    public void DecreaseResources()
    {
        startResources--;
        if (startResources < 1)
            startResources = 3;
        SetResource(startResources);
    }

    public void SetResource(int number)
    {
        if (number == 1)
            resources.text = "Standard";
        if (number == 2)
            resources.text = "Plenty";
        if (number == 3)
            resources.text = "High";
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
    }

    public void TeamColorPlayer1()
    {
        p1Color++;
        if (p1Color >= 5)
            p1Color = 1;
        SwitchColor(p1Color, teamImageP1);
    }

    public void TeamColorPlayer2()
    {
        p2Color++;
        if (p2Color >= 5)
            p2Color = 1;
        SwitchColor(p2Color, teamImageP2);
    }

    public void TeamColorPlayer3()
    {
        p3Color++;
        if (p3Color >= 5)
            p3Color = 1;
        SwitchColor(p3Color, teamImageP3);
    }

    public void TeamColorPlayer4()
    {
        p4Color++;
        if (p4Color >= 5)
            p4Color = 1;
        SwitchColor(p4Color, teamImageP4);
    }

    public void SwitchColor(int player, Image teamcolor)
    {
        switch (player)
        {
            case 1:
                teamcolor.color = Color.blue;
                break;
            case 2:
                teamcolor.color = Color.red;
                break;
            case 3:
                teamcolor.color = Color.green;
                break;
            case 4:
                teamcolor.color = Color.yellow;
                break;
        }
    }
}
