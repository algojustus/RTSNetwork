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
    public bool isGround;
    public bool buildStarted;
    public Vector3 buildinglocation;
    public GameObject finishedBuilding;
    void Awake()
    {
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
        Debug.Log(idleVillagers);
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

    private void DoBuild(int multiplier)
    {
        if (!buildStarted)
        {
            buildinglocation.y -= 20;
            finishedBuilding = Instantiate(finishedBuilding,buildinglocation,new Quaternion());
            buildStarted = true;
            return;
        }

        if (buildStarted)
        {
            currentBuildtime += Time.deltaTime;
            if (currentBuildtime >= secondsDone)
            {
                secondsDone++;
                finishedBuilding.transform.position += new Vector3(0,20f/buildingData.buildingtime*multiplier,0);
            }
            if (currentBuildtime >= buildingData.buildingtime)
            {
               gameObject.SetActive(false);
            }
        }
        //counter / hp einfügen, die dann hochgezählt werden, je mehr dorfbewohner desto höher wird gezählt 10 * dorfbewohner
        //add new progressbar
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
