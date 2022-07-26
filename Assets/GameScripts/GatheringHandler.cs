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

    private List<ResourceNode> GetCloseResource(ResourceClicked oldnode)
    {
        _resourceList.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(oldnode.transform.position, 15f);
        foreach (var collider in hitColliders)
        {
            if (collider.transform.GetComponent<ResourceClicked>() != null && collider.transform.GetComponent<ResourceClicked>().ResourceNode.HasResources())
            {
                _resourceList.Add(collider.transform.GetComponent<ResourceClicked>().ResourceNode);
            }
        }
        Debug.Log(_resourceList.Count);
        return _resourceList;
    }

    public static List<ResourceNode> GetResourceNode(ResourceClicked oldnode)
    {
        return instance.GetCloseResource(oldnode);
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
