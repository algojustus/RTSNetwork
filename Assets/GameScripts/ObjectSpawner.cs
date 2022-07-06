using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public static GameObject SpawnObject(string prefabname, Vector3 position,Quaternion rotation)
    {
        GameObject spawnedObject = Instantiate(Resources.Load(prefabname), position, rotation) as GameObject;
        return spawnedObject;
    }
}
