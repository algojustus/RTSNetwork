using UnityEngine;

public class BuildingController : MonoBehaviour
{
    private UnitController _unitController;
    private Vector3 mousePos;
    private GameObject currentBuilding;
    private bool buildingMode;
    private string currentBuildingPrefabName;
    private string teamcolor;
    public Canvas buildings;
    public Canvas villagerClicked;
    private void Start()
    {
         _unitController = transform.GetComponent<UnitController>();
         GetMyTeamColor();
    }

    private void GetMyTeamColor()
    {
        Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID].GetMyTeamColor();
        teamcolor = Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
            .color;
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
            
            mousePos.y += 3.5f;

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
        currentBuildingPrefabName = "haus_bau"+teamcolor;
        currentBuilding = Instantiate(Resources.Load(currentBuildingPrefabName),mousePos,new Quaternion()) as GameObject;
        buildingMode = true;
    }
    
    public void InstantiateBuildingModeWoodCutter()
    {
        currentBuildingPrefabName = "ressourcen_bau"+teamcolor;
        currentBuilding = Instantiate(Resources.Load(currentBuildingPrefabName),mousePos,new Quaternion()) as GameObject;
        buildingMode = true;
    }
    public void InstantiateBuildingModeStoneCutter()
    {
        currentBuildingPrefabName = "ressourcen_bau"+teamcolor;
        currentBuilding = Instantiate(Resources.Load(currentBuildingPrefabName),mousePos,new Quaternion()) as GameObject;
        buildingMode = true;
    }
    public void InstantiateBuildingModeBarracks()
    {
        currentBuildingPrefabName = "kaserne_bau"+teamcolor;    
        currentBuilding = Instantiate(Resources.Load(currentBuildingPrefabName),mousePos,new Quaternion()) as GameObject;
        buildingMode = true;
    }
    public void InstantiateBuildingModeTownCenter()
    {
        currentBuildingPrefabName = "tc_bau"+teamcolor;   
        currentBuilding = Instantiate(Resources.Load(currentBuildingPrefabName),mousePos,new Quaternion()) as GameObject;
        buildingMode = true;
    }
}
