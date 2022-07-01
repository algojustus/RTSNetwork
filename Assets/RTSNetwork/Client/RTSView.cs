using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSView : MonoBehaviour
{
    private RTSCommunicator _rtsCommunicator;
    private bool currentlySyncing = false;
    public bool syncPosition;
    public bool syncRotation;
    public bool syncScale;


    private GameObject syncedObject;
    private Vector3 position;
    private Quaternion rotation;
    private Vector3 scale;

    private int unit_id;

    void Awake()
    {
        _rtsCommunicator = GameObject.Find("RTSConnector").GetComponent<RTSCommunicator>();
        syncedObject = gameObject;
        position = syncedObject.transform.position;
        rotation = syncedObject.transform.rotation;
        scale = syncedObject.transform.localScale;

        unit_id = Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
            .unitcounter;
        Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID].UnitCounter();
        Client.serverlist.ServerlistDictionary[Client.myCurrentServer].PlayerDictionary[Client.clientID]
            .AddUnit(unit_id, "Unit", position, rotation, 100, 15, 15, 15);
        //Später vlt aus der Unit rausladen, den Prefab namen
    }

    public void Update()
    {
        if (currentlySyncing)
            SendSyncToView();
    }

    public void SendSyncToView()
    {
        _rtsCommunicator.TransferSyncedObjects(unit_id, syncedObject);
    }

    public void EnableView()
    {
        currentlySyncing = true;
    }

    public void DisableView()
    {
        currentlySyncing = false;
    }
}