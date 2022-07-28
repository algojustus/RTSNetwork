using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GatheringHandler : MonoBehaviour
{
    private static GatheringHandler instance;
    [SerializeField] private Transform storageTransform;

    private List<ResourceNode> _resourceList;
    private Dictionary<Resourcetype, List<GameObject>> storages;

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
        InitStoragesDictionary();
    }

    private void InitStoragesDictionary()
    {
        Debug.Log("I woke up");
        storages.Add(Resourcetype.Gold, new List<GameObject>());
        storages.Add(Resourcetype.Stone, new List<GameObject>());
        storages.Add(Resourcetype.Wood, new List<GameObject>());
        storages.Add(Resourcetype.Food, new List<GameObject>());
        foreach (var kvp in storages)
        {
            Debug.Log(kvp.Key);
        }
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
        Debug.Log("setting close storage");
        float olddist = 10000f;
        float dist;
        foreach (var kvp in storages)
        {
            Debug.Log("checking key: " + kvp.Key);
            if (kvp.Key != resourcetype)
                continue;
            Debug.Log("found storages: " + kvp.Value.Count);
            foreach (var storage in kvp.Value)
            {
                dist = Vector3.Distance(gatherer.position, storage.transform.position);
                Debug.Log("distance to first object " + dist);
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
        Debug.Log("getting closest storage");
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
        Debug.Log(gameObject.name + "Was added");
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
