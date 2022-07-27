using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GatheringUnit : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    private UnitSelected unitControlls;
    private State state;
    public ResourceNode resourceNode;
    private Transform storage;
    private int resourceInventoryAmount=0;
    private bool idleGatherer;
    private Vector3 movepos;
    public ResourcesUI resourceUi;
    private List<ResourceNode> _resourceList;
    
    private void Awake()
    {
        unitControlls = gameObject.GetComponent<UnitSelected>();
        state = State.Idle;
        resourceUi = GameObject.Find("GameManager").GetComponent<ResourcesUI>();
        _resourceList = new List<ResourceNode>();
    }

    private enum State
    {
        Idle,
        MovingToResource,
        Gathering,
        MovingToStorage,
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                if(resourceNode != null)
                {
                    state = State.MovingToResource;
                }
                break;
            case State.MovingToResource:
                if (unitControlls.isIdle)
                {
                    MoveToResource(
                        resourceNode.GetPosition(), 5f, () =>
                        {
                            state = State.Gathering;
                        });
                }
                break;
            case State.Gathering:
                if (unitControlls.isIdle)
                {
                    storage = GatheringHandler.GetStorage();
                    if (!resourceNode.HasResources())
                    {
                           state = State.MovingToStorage;
                           break;
                    }
                    
                    if (resourceInventoryAmount >= 10)
                    {
                        state = State.MovingToStorage;
                    } else
                    {
                        StartGatheringEffect(() => {
                            resourceInventoryAmount++;
                            resourceNode.ReduceResourceCount();
                            unitControlls.isIdle = false;
                            unitControlls.currentIdleTime = 0;
                        });
                    }
                }
                break;
            case State.MovingToStorage:
                if (unitControlls.isIdle)
                {

                    if (resourceNode == null)
                    {
                        state = State.Idle;
                        return;
                    }  

                    if (!resourceNode.HasResources())
                    {
                        //var tempnode = resourceNode;
                        _resourceList = GatheringHandler.GetResourceNode(resourceNode.node.GetComponent<ResourceClicked>());
                        GetNextCloseResource();
                    }
                    MoveToStorage(
                        storage.position, () =>
                        {
                            resourceUi.AddFood(resourceInventoryAmount);
                            resourceInventoryAmount = 0;
                            state = State.Idle;
                        });
                }
                break;
        }
    }


    private void GetNextCloseResource()
    {
        if (_resourceList.Count <= 0)
        {
            Destroy(resourceNode.node.gameObject);
            resourceNode = null;
            return;
        }
        Destroy(resourceNode.node.gameObject);
        resourceNode = null;
        resourceNode = _resourceList[0];
        _resourceList.Remove(resourceNode);
    }
    private void MoveToResource(Vector3 pos, float stopDistance, Action onArrival)
    {
        movepos = pos;
        movepos.x -= stopDistance;
        movepos.y -= stopDistance;
        unitControlls.MoveToPosition(movepos);
        
        if(movepos.x == transform.position.x && movepos.z == transform.position.z)
            onArrival.Invoke();
    }
    
    private void MoveToStorage(Vector3 pos , Action onArrival)
    {
        movepos = pos;
        unitControlls.MoveToPosition(movepos);
        
        if(movepos.x == transform.position.x && movepos.z == transform.position.z)
            onArrival.Invoke();
    }
    
    private void StartGatheringEffect(Action onStart)
    {
        particles.Play();
        onStart.Invoke();
    }
}
