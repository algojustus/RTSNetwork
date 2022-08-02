using UnityEngine;

public class ResourceClicked : MonoBehaviour
{
    public ResourceNode ResourceNode;
    public int ResourceAmount = 3;
    public GatheringHandler.Resourcetype resourceType;
    public int resourceID;

    public GatheringHandler.Resourcetype GetResourceType()
    {
        return resourceType;
    }

    private void Awake()
    {
        ResourceNode = new ResourceNode(transform, ResourceAmount);
        resourceID = GatheringHandler.ReceiveResourceID(gameObject);
    }
}
