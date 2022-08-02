using System;
using System.Collections.Generic;
using UnityEngine;

public class GatheringUnit : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    private List<ResourceNode> _resourceList;
    private UnitSelected unitControlls;
    public GatheringHandler.Resourcetype resource;
    public ResourceNode resourceNode;
    public ResourcesUI resourceUi;
    private Transform storage;
    public State state;
    public State movement;
    private Vector3 movepos;
    private Vector3 tempNodeLoc;
    private int resourceInventoryAmount = 0;
    private bool idleGatherer;

    private void Awake()
    {
        unitControlls = gameObject.GetComponent<UnitSelected>();
        state = State.Idle;
        resourceUi = GameObject.Find("GameManager").GetComponent<ResourcesUI>();
        _resourceList = new List<ResourceNode>();
    }

    public enum State
    {
        Idle,
        MovingToResource,
        Gathering,
        MovingToStorage,
        Movement,
    }

    void Update()
    {
        try
        {
            switch (state)
            {
                case State.Idle:
                    if (movement == State.Movement)
                    {
                        resourceNode = null;
                        return;
                    }

                    if (resourceNode != null)
                    {
                        state = State.MovingToResource;
                        tempNodeLoc = resourceNode.GetPosition();
                        resource = resourceNode.node.GetComponent<ResourceClicked>().GetResourceType();
                        storage = GatheringHandler.GetStorage(
                            resourceNode.node.GetComponent<ResourceClicked>().GetResourceType(),
                            transform);
                    }

                    break;
                case State.MovingToResource:

                    if (movement == State.Movement)
                    {
                        resourceNode = null;
                        return;
                    }

                    if (unitControlls.isIdle)
                    {
                        if (resourceNode == null)
                        {
                            state = State.Idle;
                            return;
                        }

                        MoveToResource(
                            tempNodeLoc,
                            5f,
                            () =>
                            {
                                state = State.Gathering;
                            });
                    }

                    break;
                case State.Gathering:
                    if (movement == State.Movement)
                    {
                        resourceNode = null;
                        return;
                    }

                    if (unitControlls.isIdle)
                    {
                        if (resourceInventoryAmount >= 10 || !resourceNode.HasResources())
                        {
                            state = State.MovingToStorage;
                        } else
                        {
                            StartGatheringEffect(
                                () =>
                                {
                                    resourceInventoryAmount++;
                                    resourceNode.ReduceResourceCount();
                                    unitControlls.isIdle = false;
                                    unitControlls.currentIdleTime = 0;
                                });
                        }
                    }

                    break;
                case State.MovingToStorage:
                    if (movement == State.Movement)
                    {
                        resourceNode = null;
                        return;
                    }

                    if (unitControlls.isIdle)
                    {
                        if (resourceNode == null)
                        {
                            state = State.Idle;
                            return;
                        }

                        if (!resourceNode.HasResources())
                        {
                            if (resourceNode == null)
                            {
                                resourceNode = _resourceList[0];
                                state = State.Idle;
                                return;
                            }

                            _resourceList =
                                GatheringHandler.GetResourceNode(resourceNode.node.GetComponent<ResourceClicked>());
                            GetNextCloseResource();
                        }

                        MoveToStorage(
                            storage.position,
                            () =>
                            {
                                AddCollectedResource(resource);
                                resourceInventoryAmount = 0;
                                state = State.Idle;
                            });
                    }

                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            resourceNode = null;
            state = State.Idle;
            throw;
        }
    }


    private void AddCollectedResource(GatheringHandler.Resourcetype resourcetype)
    {
        switch (resourcetype)
        {
            case GatheringHandler.Resourcetype.Food:
                resourceUi.AddFood(resourceInventoryAmount);
                break;
            case GatheringHandler.Resourcetype.Stone:
                resourceUi.AddStone(resourceInventoryAmount);
                break;
            case GatheringHandler.Resourcetype.Wood:
                resourceUi.AddWood(resourceInventoryAmount);
                break;
            case GatheringHandler.Resourcetype.Gold:
                resourceUi.AddGold(resourceInventoryAmount);
                break;
        }
    }

    private void GetNextCloseResource()
    {
        ClientMessages.DestroyResource(Client.myCurrentServer, resourceNode.node.name);
        GatheringHandler.RemoveResource(resourceNode.node.name);
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

        if (movepos.x == transform.position.x && movepos.z == transform.position.z)
            onArrival.Invoke();
    }

    private void MoveToStorage(Vector3 pos, Action onArrival)
    {
        movepos = pos;
        unitControlls.MoveToPosition(movepos);

        if (movepos.x == transform.position.x && movepos.z == transform.position.z)
            onArrival.Invoke();
    }

    private void StartGatheringEffect(Action onStart)
    {
        particles.Play();
        onStart.Invoke();
    }
}
