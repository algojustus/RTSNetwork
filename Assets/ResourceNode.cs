using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode
{
   private int resourceCount;
   private Transform node;
   public ResourceNode(Transform resource, int resourceCount)
   {
      node = resource;
      SetResourceCount(resourceCount);
   }

   public Vector3 GetPosition()
   {
      return node.position;
   }

   public void SetResourceCount(int count)
   {
      resourceCount = count;
   }
   public void ReduceResourceCount()
   {
      resourceCount--;
      Debug.Log(resourceCount +" left");
   }

   public bool HasResources()
   {
      return resourceCount > 0;
   }
}
