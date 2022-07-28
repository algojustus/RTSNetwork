using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingSelected : MonoBehaviour
{
    public Building buildingData;
    public GameObject _selectedGameObject;
    private Dictionary<int, GameObject> unitsEntered;
    private Dictionary<int, Vector3> unitsoldpos;
    private float currentBuildtime = 0;
    private float currentIdleTime = 0;
    private int secondsDone = 1;
    public int buildinghp = 0;
    public bool isGround;
    public bool buildStarted;
    public Vector3 buildinglocation;
    public GameObject finishedBuilding;
    public ResourcesUI resourceUi;
    public int building_id;

    void Awake()
    {
        resourceUi = GameObject.Find("GameManager").GetComponent<ResourcesUI>();
        _selectedGameObject = transform.Find("Selected").gameObject;
        SetSelectedVisible(false);
        unitsEntered = new Dictionary<int, GameObject>();
        unitsoldpos = new Dictionary<int, Vector3>();
        buildinglocation = transform.position;
    }

    private void Update()
    {
        if(unitsEntered.Count <= 0)
            return;
        
        OnStay();
        int idleVillagers = unitsEntered.Count(kvp => kvp.Value.transform.GetComponent<UnitSelected>().isIdle);
        
        if (idleVillagers <= 0)
            return;
        
        DoBuild(idleVillagers);
    }

    public void SetSelectedVisible(bool visible)
    {
        _selectedGameObject.SetActive(visible);
    }

    private void OnStay()
    {
        foreach (var kvp in unitsEntered)
        {
            if (kvp.Value.transform.position == unitsoldpos[kvp.Key])
            {
                StayIdle(kvp.Value);
            } else
            {
                kvp.Value.transform.GetComponent<UnitSelected>().isIdle = false;
                currentIdleTime = 0;
                unitsoldpos[kvp.Key] = unitsEntered[kvp.Key].transform.position;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (finishedBuilding == null)
            return;
        if (!collision.transform.CompareTag("player1_villager"))
            return;
        unitsEntered.Add(collision.transform.GetComponent<RTSView>().unit_id, collision.gameObject);
        unitsoldpos.Add(collision.transform.GetComponent<RTSView>().unit_id, collision.transform.position);
    }

    private void OnTriggerExit(Collider collision)
    {
        if (finishedBuilding == null)
            return;
        if (!collision.transform.CompareTag("player1_villager"))
            return;
        unitsEntered.Remove(collision.transform.GetComponent<RTSView>().unit_id);
        unitsoldpos.Remove(collision.transform.GetComponent<RTSView>().unit_id);
        currentIdleTime = 0;
    }

    public void DoBuild(int multiplier)
    {
        if (!buildStarted)
        {
            buildinglocation.y -= 20;
            finishedBuilding = Instantiate(finishedBuilding,buildinglocation,new Quaternion());
            finishedBuilding.GetComponent<BuildingSelected>().enabled = false;
            buildStarted = true;
            ClientMessages.BuildingInit(Client.myCurrentServer, Client.otherID,building_id, multiplier, true,false);
            return;
        }

        if (buildStarted)
        {
            currentBuildtime += Time.deltaTime;
            if (currentBuildtime >= secondsDone)
            {
                ClientMessages.BuildingInit(Client.myCurrentServer, Client.otherID,building_id, multiplier, false,false);
                secondsDone++;
                buildinghp += 1 *multiplier;
                if (buildinghp >= buildingData.buildingtime)
                {
                    int leftover = buildingData.buildingtime - buildinghp;
                    finishedBuilding.transform.position += new Vector3(0,20f/buildingData.buildingtime*leftover,0);
                    gameObject.SetActive(false);
                    ClientMessages.BuildingInit(Client.myCurrentServer, Client.otherID,building_id, 1, false,true);
                    finishedBuilding.GetComponent<BuildingSelected>().enabled = true;
                    if (finishedBuilding.name.Contains("stein"))
                    {
                        GatheringHandler.AddStorages(GatheringHandler.Resourcetype.Stone,finishedBuilding);
                        GatheringHandler.AddStorages(GatheringHandler.Resourcetype.Gold,finishedBuilding);
                    } 
                    if (finishedBuilding.name.Contains("holz"))
                    {
                        GatheringHandler.AddStorages(GatheringHandler.Resourcetype.Wood,finishedBuilding);
                    } 
                    if (finishedBuilding.name.Contains("tc"))
                    {
                        GatheringHandler.AddTCStorages(finishedBuilding);
                    } 
                    if (finishedBuilding.name.Contains("haus") || finishedBuilding.name.Contains("tc"))
                    {
                        if (resourceUi.villager_max >= resourceUi.villager_cap)
                            return;
                        resourceUi.villager_max += 5;
                        resourceUi.villager_ui.text = resourceUi.villager_count + "|" + resourceUi.villager_max;
                    }
                    return;
                }
                finishedBuilding.transform.position += new Vector3(0,20f/buildingData.buildingtime*multiplier,0);
            }
        }
        //TODO:: new progressbar
    }

    public GameObject InitBuildingMultiplayer()
    {
        buildinglocation.y -= 21;
        finishedBuilding = Instantiate(finishedBuilding,buildinglocation,new Quaternion());
        finishedBuilding.GetComponent<BuildingSelected>().enabled = false;
        return finishedBuilding;
    }
    private void StayIdle(GameObject unit)
    {
        currentIdleTime += Time.deltaTime;
        if (currentIdleTime >= 2f)
        {
            unit.GetComponent<UnitSelected>().isIdle = true;
        }
    }
}
