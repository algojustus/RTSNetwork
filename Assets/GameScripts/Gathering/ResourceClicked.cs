using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceClicked : MonoBehaviour
{
    public ResourceNode ResourceNode;
    public int ResourceAmount = 3;
    public GatheringHandler.Resourcetype resourceType;
    
    public Transform ReturnOnClicked()
    {
        return transform;
    }


    public GatheringHandler.Resourcetype GetResourceType()
    {
        return resourceType;
    }
    private void Awake()
    {
        ResourceNode = new ResourceNode(transform, ResourceAmount);
    }
}
