using System.Collections.Generic;
using UnityEngine;

public class GatheringHandler : MonoBehaviour
{
    private static GatheringHandler instance;
    [SerializeField] private Transform storageTransform;

    private List<ResourceNode> _resourceList;
    private Dictionary<Resourcetype, List<GameObject>> storages;
    private Dictionary<int, GameObject> availableResources;
    private int resourceCounter = 0;
    public enum Resourcetype
    {
        Gold,
        Wood,
        Stone,
        Food,
    }

    private void Awake()
    {
        instance = this;
        _resourceList = new List<ResourceNode>();
        storages = new Dictionary<Resourcetype, List<GameObject>>();
        availableResources = new Dictionary<int, GameObject>();
        InitStoragesDictionary();
    }

    private int InitResource(GameObject resource)
    {
        resourceCounter++;
        availableResources.Add(resourceCounter,resource);
        return resourceCounter;
    }
    public static int ReceiveResourceID(GameObject resource)
    {
        return instance.InitResource(resource);
    }
    private void DestroyResource(string resource_id)
    {
        foreach (var name in availableResources)
        {
            if (name.Value.name == resource_id)
            {
                GameObject tempresource = availableResources[name.Key];
                Destroy(tempresource);
                availableResources.Remove(name.Key);
                break;
            }
        }
    }
    
    public static void RemoveResource(string resource_id)
    {
        instance.DestroyResource(resource_id);
    }
    
    private void InitStoragesDictionary()
    {
        storages.Add(Resourcetype.Gold, new List<GameObject>());
        storages.Add(Resourcetype.Stone, new List<GameObject>());
        storages.Add(Resourcetype.Wood, new List<GameObject>());
        storages.Add(Resourcetype.Food, new List<GameObject>());
    }

    private List<ResourceNode> GetCloseResource(ResourceClicked oldnode)
    {
        _resourceList.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(oldnode.transform.position, 15f);
        foreach (var collider in hitColliders)
        {
            if (collider.transform.GetComponent<ResourceClicked>() != null &&
                collider.transform.GetComponent<ResourceClicked>().ResourceNode.HasResources())
            {
                _resourceList.Add(collider.transform.GetComponent<ResourceClicked>().ResourceNode);
            }
        }

        return _resourceList;
    }

    public static List<ResourceNode> GetResourceNode(ResourceClicked oldnode)
    {
        return instance.GetCloseResource(oldnode);
    }

    private Transform GetClosestStorage(Resourcetype resourcetype, Transform gatherer)
    {
        float olddist = 10000f;
        float dist;
        foreach (var kvp in storages)
        {
            if (kvp.Key != resourcetype)
                continue;
            foreach (var storage in kvp.Value)
            {
                dist = Vector3.Distance(gatherer.position, storage.transform.position);
                if (olddist > dist)
                {
                    olddist = dist;
                    storageTransform = storage.transform;
                }
            }
        }
        return storageTransform;
    }

    public static Transform GetStorage(Resourcetype resourcetype, Transform gatherer)
    {
        return instance.GetClosestStorage(resourcetype, gatherer);
    }

    private void AddStorage(Resourcetype resourcetype, GameObject gameObject)
    {
        storages[resourcetype].Add(gameObject);
    }

    public static void AddStorages(Resourcetype resourcetype, GameObject gameObject)
    {
        instance.AddStorage(resourcetype, gameObject);
    }

    private void AddTCStorage(GameObject gameObject)
    {
        storages[Resourcetype.Food].Add(gameObject);
        storages[Resourcetype.Stone].Add(gameObject);
        storages[Resourcetype.Wood].Add(gameObject);
        storages[Resourcetype.Gold].Add(gameObject);
    }

    public static void AddTCStorages(GameObject gameObject)
    {
        instance.AddTCStorage(gameObject);
    }
}
