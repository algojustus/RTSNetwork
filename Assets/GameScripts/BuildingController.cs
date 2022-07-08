using System;
using UnityEngine;
using UnityEngine.UI;

public class BuildingController : MonoBehaviour
{
    private UnitController _unitController;
    private Vector3 mousePos;
    private GameObject currentBuilding;
    private bool buildingMode;
    private string currentBuildingPrefabName;
    public Canvas buildings;
    public Canvas villagerClicked;
    public Camera cameraStartPos;

    private void Start()
    {
         _unitController = transform.GetComponent<UnitController>();
         SpawnInitTowncenter();
    }

    private void SpawnInitTowncenter()
    {
        Vector3 startCord = new Vector3(1,1,1);
        int building_id;
        
        if(Client.myGameColor == "_blau")
            startCord = new Vector3(-111,0,-12.5f);
        if(Client.myGameColor == "_rot")
            startCord = new Vector3(-5,0,-14);

        cameraStartPos.transform.position = startCord;
        GameObject building = Instantiate(Resources.Load("tc"+Client.myGameColor),startCord,new Quaternion()) as GameObject;
        building_id = Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
            .buildingcounter;
        Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
            .BuildingCounter();
        Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
            .AddBuilding(building_id,"tc"+Client.myGameColor,startCord,new Quaternion(),building);
    }
    public void SetUiOnBuilding(Building building, BuildingSelected buildingSelected)
    {
        int unitCount = 1;
        int technologyCount = 1;
        foreach (var units in building.spawnables)
        {
            var unit = buildings.transform.GetChild(0).Find("Unit" + unitCount);
             Debug.Log(unit);
            unit.gameObject.SetActive(true);
            if(units == "villager")
                unit.GetComponent<Button>().onClick.AddListener(delegate { _unitController.InstantiateVillager(buildingSelected);});
            if(units == "spear")
                unit.GetComponent<Button>().onClick.AddListener(delegate { _unitController.InstantiateSpear(buildingSelected);});
            if(units == "sword")
                unit.GetComponent<Button>().onClick.AddListener(delegate { _unitController.InstantiateSword(buildingSelected);});
            if(units == "bow")
                unit.GetComponent<Button>().onClick.AddListener(delegate { _unitController.InstantiateBow(buildingSelected);});
            unitCount++;
        }
        foreach (var units in building.technologies)
        {
            // var tech = buildings.transform.GetChild(0).Find("Unit" + unitCount);
            // tech.gameObject.SetActive(true);
            //Add techs later
            technologyCount++;
        }

        unitCount = 1;
        technologyCount = 1;
    }
    public void ResetBuildingUI(Building building, BuildingSelected buildingSelected)
    {
        int unitCount = 1;
        int technologyCount = 1;
        foreach (var units in building.spawnables)
        {
            var unit = buildings.transform.GetChild(0).Find("Unit" + unitCount);
            if(units == "villager")
                unit.GetComponent<Button>().onClick.RemoveListener(delegate { _unitController.InstantiateVillager(buildingSelected);});
            if(units == "spear")
                unit.GetComponent<Button>().onClick.RemoveListener(delegate { _unitController.InstantiateSpear(buildingSelected);});
            if(units == "sword")
                unit.GetComponent<Button>().onClick.RemoveListener(delegate { _unitController.InstantiateSword(buildingSelected);});
            if(units == "bow")
                unit.GetComponent<Button>().onClick.RemoveListener(delegate { _unitController.InstantiateBow(buildingSelected);});
            unit.gameObject.SetActive(false);
            unitCount++;
        }
        foreach (var units in building.technologies)
        {
            // var tech = buildings.transform.GetChild(0).Find("Unit" + unitCount);
            // tech.gameObject.SetActive(false);
            // technologyCount++;
        }

        unitCount = 1;
        technologyCount = 1;
    }
    
    private void Update()
    {
        if (currentBuilding != null)
        {
            mousePos = _unitController.CheckWhereMouseClicked();
            currentBuilding.transform.position = mousePos;
        }

        if (buildingMode && Input.GetMouseButtonDown(0))
        {
            int building_id;
            GameObject building = Instantiate(Resources.Load(currentBuildingPrefabName),mousePos,new Quaternion()) as GameObject;
            Destroy(currentBuilding);
            buildingMode = false;
            
            mousePos.y += 3f;

            building_id = Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .buildingcounter;
            Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .BuildingCounter();
            Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .AddBuilding(building_id,currentBuildingPrefabName,mousePos,new Quaternion(),building);
        }
        
        if (buildingMode && Input.GetMouseButtonDown(1))
        {
            Destroy(currentBuilding);
            currentBuildingPrefabName = "";
            buildingMode = false;
        }
    }

    public void InstantiateBuildingModeHouse()
    {
        currentBuildingPrefabName = "haus_bau"+Client.myGameColor;
        currentBuilding = Instantiate(Resources.Load(currentBuildingPrefabName),mousePos,new Quaternion()) as GameObject;
        buildingMode = true;
    }
    
    public void InstantiateBuildingModeWoodCutter()
    {
        currentBuildingPrefabName = "ressourcen_bau"+Client.myGameColor;
        currentBuilding = Instantiate(Resources.Load(currentBuildingPrefabName),mousePos,new Quaternion()) as GameObject;
        buildingMode = true;
    }
    public void InstantiateBuildingModeStoneCutter()
    {
        currentBuildingPrefabName = "ressourcen_bau"+Client.myGameColor;
        currentBuilding = Instantiate(Resources.Load(currentBuildingPrefabName),mousePos,new Quaternion()) as GameObject;
        buildingMode = true;
    }
    public void InstantiateBuildingModeBarracks()
    {
        currentBuildingPrefabName = "kaserne_bau"+Client.myGameColor;    
        currentBuilding = Instantiate(Resources.Load(currentBuildingPrefabName),mousePos,new Quaternion()) as GameObject;
        buildingMode = true;
    }
    public void InstantiateBuildingModeTownCenter()
    {
        currentBuildingPrefabName = "tc_bau"+Client.myGameColor;   
        currentBuilding = Instantiate(Resources.Load(currentBuildingPrefabName),mousePos,new Quaternion()) as GameObject;
        buildingMode = true;
    }
    
    public void InstantiateBuildingModeTownCenterDone()
    {
        currentBuildingPrefabName = "tc"+Client.myGameColor;   
        currentBuilding = Instantiate(Resources.Load(currentBuildingPrefabName),mousePos,new Quaternion()) as GameObject;
        buildingMode = true;
    }
}
