using UnityEngine;
using UnityEngine.UI;

public class BuildingController : MonoBehaviour
{
    private UnitController _unitController;
    private ResourcesUI _resources;
    private GameObject currentBuilding;
    public Canvas buildings;
    public Canvas villagerClicked;
    public Camera cameraStartPos;
    private Vector3 mousePos;
    private bool buildingMode;
    private string currentBuildingPrefabName;

    private void Start()
    {
        _unitController = transform.GetComponent<UnitController>();
        _resources = transform.GetComponent<ResourcesUI>();
        SpawnInitTowncenter();
    }

    private void SpawnInitTowncenter()
    {
        Vector3 startCord = new Vector3(1, 1, 1);
        int building_id;

        if (Client.myGameColor == "_blau")
            startCord = new Vector3(-111, 0, -12.5f);
        if (Client.myGameColor == "_rot")
            startCord = new Vector3(120, 0, -12.5f);

        cameraStartPos.transform.position = new Vector3(startCord.x, cameraStartPos.transform.position.y, startCord.z);
        GameObject building =
            Instantiate(Resources.Load("tc" + Client.myGameColor), startCord, new Quaternion()) as GameObject;
        building_id = Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
            .buildingcounter;
        Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
            .BuildingCounter();
        Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
            .AddBuilding(building_id, "tc" + Client.myGameColor, startCord, new Quaternion(), building);
        GatheringHandler.AddTCStorages(building);
    }

    public void SetUiOnBuilding(Building building, BuildingSelected buildingSelected)
    {
        int unitCount = 1;
        foreach (var units in building.spawnables)
        {
            var unit = buildings.transform.GetChild(0).Find("Unit" + unitCount);
            unit.gameObject.AddComponent<Button>();
            unit.GetComponent<Image>().sprite = building.sprites[unitCount - 1];
            unit.gameObject.SetActive(true);
            if (units == "villager")
                unit.GetComponent<Button>().onClick.AddListener(
                    delegate
                    {
                        _unitController.InitVillager(buildingSelected);
                    });
            if (units == "spear")
                unit.GetComponent<Button>().onClick.AddListener(
                    delegate
                    {
                        _unitController.InitSpear(buildingSelected);
                    });
            if (units == "sword")
                unit.GetComponent<Button>().onClick.AddListener(
                    delegate
                    {
                        _unitController.InitSword(buildingSelected);
                    });
            if (units == "bow")
                unit.GetComponent<Button>().onClick.AddListener(
                    delegate
                    {
                        _unitController.InitBow(buildingSelected);
                    });
            unitCount++;
        }
    }

    public void ResetBuildingUI(Building building, BuildingSelected buildingSelected)
    {
        int unitCount = 1;
        foreach (var units in building.spawnables)
        {
            var unit = buildings.transform.GetChild(0).Find("Unit" + unitCount);
            Destroy(unit.gameObject.GetComponent<Button>());
            unit.gameObject.SetActive(false);
            unitCount++;
        }
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
            GameObject building =
                Instantiate(Resources.Load(currentBuildingPrefabName), mousePos, new Quaternion()) as GameObject;
            Destroy(currentBuilding);
            buildingMode = false;

            mousePos.y += 3f;

            building_id = Client.serverlist.ServerlistDictionary[Client.myCurrentServer]
                .PlayerDictionary[Client.clientID]
                .buildingcounter;
            Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .BuildingCounter();
            Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
                .AddBuilding(building_id, currentBuildingPrefabName, mousePos, new Quaternion(), building);
            building.GetComponent<BuildingSelected>().building_id = building_id;
            _resources.BuildWithResources(currentBuildingPrefabName);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Screen.SetResolution(960, 540, false, 60);
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
        bool allowedToBuild = _resources.HasEnoughResources(0, 60, 0, 0);
        if (allowedToBuild)
        {
            currentBuildingPrefabName = "haus_bau" + Client.myGameColor;
            currentBuilding =
                Instantiate(Resources.Load(currentBuildingPrefabName), mousePos, new Quaternion()) as GameObject;
            buildingMode = true;
        }
    }

    public void InstantiateBuildingModeWoodCutter()
    {
        bool allowedToBuild = _resources.HasEnoughResources(0, 100, 0, 0);
        if (allowedToBuild)
        {
            currentBuildingPrefabName = "holz_bau" + Client.myGameColor;
            currentBuilding =
                Instantiate(Resources.Load(currentBuildingPrefabName), mousePos, new Quaternion()) as GameObject;
            buildingMode = true;
        }
    }

    public void InstantiateBuildingModeStoneCutter()
    {
        bool allowedToBuild = _resources.HasEnoughResources(0, 100, 0, 0);
        if (allowedToBuild)
        {
            currentBuildingPrefabName = "stein_bau" + Client.myGameColor;
            currentBuilding =
                Instantiate(Resources.Load(currentBuildingPrefabName), mousePos, new Quaternion()) as GameObject;
            buildingMode = true;
        }
    }

    public void InstantiateBuildingModeBarracks()
    {
        bool allowedToBuild = _resources.HasEnoughResources(0, 150, 0, 0);
        if (allowedToBuild)
        {
            currentBuildingPrefabName = "kaserne_bau" + Client.myGameColor;
            currentBuilding =
                Instantiate(Resources.Load(currentBuildingPrefabName), mousePos, new Quaternion()) as GameObject;
            buildingMode = true;
        }
    }

    public void InstantiateBuildingModeTownCenter()
    {
        bool allowedToBuild = _resources.HasEnoughResources(0, 275, 0, 150);
        if (allowedToBuild)
        {
            currentBuildingPrefabName = "tc_bau" + Client.myGameColor;
            currentBuilding =
                Instantiate(Resources.Load(currentBuildingPrefabName), mousePos, new Quaternion()) as GameObject;
            buildingMode = true;
        }
    }

    public void InstantiateBuildingModeTownCenterDone()
    {
        currentBuildingPrefabName = "tc" + Client.myGameColor;
        currentBuilding =
            Instantiate(Resources.Load(currentBuildingPrefabName), mousePos, new Quaternion()) as GameObject;
        buildingMode = true;
    }
}
