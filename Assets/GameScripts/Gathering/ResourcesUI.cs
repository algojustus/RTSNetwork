using UnityEngine;
using UnityEngine.UI;

public class ResourcesUI : MonoBehaviour
{
    public Canvas resourcesUI;
    public Text food_ui;
    public Text wood_ui;
    public Text gold_ui;
    public Text stone_ui;
    public Text villager_ui;
    private int food = 1250;
    private int wood = 1550;
    private int gold = 1150;
    private int stone = 1250;
    public int villager_max = 5;
    public int villager_count = 0;
    public int villager_cap = 123;

    private void Start()
    {
        switch (Client.serverlist.ServerlistDictionary[Client.myCurrentServer].start_resources)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }

        food_ui.text = "" + food;
        wood_ui.text = "" + wood;
        gold_ui.text = "" + gold;
        stone_ui.text = "" + stone;
        villager_ui.text = villager_count + " | " + villager_max;
        villager_cap = Client.serverlist.ServerlistDictionary[Client.myCurrentServer].max_villagers;
        if (villager_cap <= 0)
            villager_cap = 100;
    }

    public bool HasEnoughResources(int _food, int _wood, int _gold, int _stone)
    {
        bool hasEnough;
        int newFoodValue = food - _food;
        int newWoodValue = wood - _wood;
        int newGoldValue = gold - _gold;
        int newStoneValue = stone - _stone;

        if (newFoodValue < 0 || newWoodValue < 0 || newGoldValue < 0 || newStoneValue < 0)
        {
            hasEnough = false;
        } else
        {
            hasEnough = true;
        }

        return hasEnough;
    }

    public void BuildWithResources(string buildingname)
    {
        if (buildingname == "haus_bau" + Client.myGameColor)
        {
            RemoveWood(60);
        }

        if (buildingname == "ressourcen_bau" + Client.myGameColor)
        {
            RemoveWood(100);
        }

        if (buildingname == "kaserne_bau" + Client.myGameColor)
        {
            RemoveWood(150);
        }

        if (buildingname == "tc_bau" + Client.myGameColor)
        {
            RemoveWood(275);
            RemoveStone(150);
        }

        if (buildingname == "villager" + Client.myGameColor)
        {
            RemoveFood(50);
        }

        if (buildingname == "sword" + Client.myGameColor)
        {
            RemoveFood(60);
            RemoveGold(35);
        }

        if (buildingname == "spear" + Client.myGameColor)
        {
            RemoveFood(30);
            RemoveWood(25);
        }

        if (buildingname == "bow" + Client.myGameColor)
        {
            RemoveWood(35);
            RemoveGold(50);
        }
    }

    public void AddFood(int amount)
    {
        food += amount;
        food_ui.text = "" + food;
    }

    public void AddWood(int amount)
    {
        wood += amount;
        wood_ui.text = "" + wood;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        gold_ui.text = "" + gold;
    }

    public void AddStone(int amount)
    {
        stone += amount;
        stone_ui.text = "" + stone;
    }

    public void RemoveFood(int amount)
    {
        food -= amount;
        food_ui.text = "" + food;
    }

    public void RemoveWood(int amount)
    {
        wood -= amount;
        wood_ui.text = "" + wood;
    }

    public void RemoveGold(int amount)
    {
        gold -= amount;
        gold_ui.text = "" + gold;
    }

    public void RemoveStone(int amount)
    {
        stone -= amount;
        stone_ui.text = "" + stone;
    }

    public void AddVillager()
    {
        villager_count += 1;
        villager_ui.text = villager_count + " | " + villager_max;
    }

    public void RemoveVillager()
    {
        villager_count -= 1;
        villager_ui.text = villager_count + " | " + villager_max;
    }

    public void IncreaseMaxVillagers()
    {
        villager_max += 5;
        villager_ui.text = villager_count + " | " + villager_max;
    }

    public void DecreaseMaxVillagers()
    {
        villager_max -= 5;
        villager_ui.text = villager_count + " | " + villager_max;
    }
}
