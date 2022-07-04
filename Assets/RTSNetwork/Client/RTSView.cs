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
        //Später vlt aus der Unit rausladen, den Prefab namen
    }

    public void FixedUpdate()
    {
        if (currentlySyncing && syncedObject.transform.CompareTag("Player1"))
            SendSyncToView();
    }

    public void SendSyncToView()
    {
        //_rtsCommunicator.TransferSyncedObjects(unit_id, syncedObject);
    }
    public void SendMoveToPos(Vector3 moveTo)
    {
        if (syncedObject.transform.CompareTag("Player1"))
        {
            _rtsCommunicator.TransferMoveToPos(unit_id, moveTo);
        }
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