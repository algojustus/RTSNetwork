using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceClicked : MonoBehaviour
{
    public ResourceNode ResourceNode;
    public int ResourceAmount = 3;
    public Transform ReturnOnClicked()
    {
        return transform;
    }

    private void Awake()
    {
        ResourceNode = new ResourceNode(transform, ResourceAmount);
    }
}
