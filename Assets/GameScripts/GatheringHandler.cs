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

    private void Awake()
    {
        instance = this;
        _resourceList = new List<ResourceNode>();
    }

    private ResourceNode GetCloseResource()
    {
        List<ResourceNode> cloneList = new List<ResourceNode>(_resourceList);
        foreach (var node in cloneList)
        {
            if (!node.HasResources())
                _resourceList.Remove(node);
        }

        return cloneList.Count > 0 ? null : _resourceList[UnityEngine.Random.Range(0,_resourceList.Count)];
    }

    public static ResourceNode GetResourceNode()
    {
        return instance.GetCloseResource();
    }
    private Transform GetClosestStorage()
    {
        return storageTransform;
    }

    public static Transform GetStorage()
    {
        return instance.GetClosestStorage();
    }
    private void SetClosestStorage(GameObject gameObject)
    {
        storageTransform = gameObject.transform;
    }

    public static void SetStorage(GameObject gameObject)
    {
        instance.SetClosestStorage(gameObject);
    }
}
