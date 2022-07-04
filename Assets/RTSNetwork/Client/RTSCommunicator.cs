using UnityEngine;

public class RTSCommunicator : MonoBehaviour
{
    private GameObject sendObjects;

    public void TransferSyncedObjects(int unit_id, GameObject gameObjects)
    {
        sendObjects = gameObjects;
        ClientMessages.BroadCastUnitPosition(unit_id,Client.myCurrentServer, Client.otherID,sendObjects.transform.position);
    }
    public void TransferMoveToPos(int unit_id, Vector3 moveTo)
    {
        ClientMessages.BroadCastUnitPosition(unit_id,Client.myCurrentServer, Client.otherID,moveTo);
    }
}