using UnityEngine;

public class RTSCommunicator : MonoBehaviour
{
    //private List<GameObject> syncedObjects = new List<GameObject>();
    private GameObject sendObjects;
    
    void Update()
    {
        //if (syncedObjects.Count > 0) 
            //SendMessage();
    }

    public void TransferSyncedObjects(int unit_id, GameObject gameObjects)
    {
        sendObjects = gameObjects;
        SendMessage(unit_id);
        //syncedObjects.Add(sendObjects);
    }

    private void SendMessage(int unitID)
    {
        // foreach (var sync in syncedObjects)
        // {
            ClientMessages.BroadCastUnits(unitID,Client.myCurrentServer, Client.otherID,sendObjects.transform.position, sendObjects.transform.rotation);
        // }
        //syncedObjects.Clear();
    }
}