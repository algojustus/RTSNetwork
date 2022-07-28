using UnityEngine;

public class ResourceNode
{
   private int resourceCount;
   public Transform node;
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
   }

   public bool HasResources()
   {
      return resourceCount > 0;
   }
}
