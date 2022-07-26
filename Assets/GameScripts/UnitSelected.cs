using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class UnitSelected : MonoBehaviour
{
    private RTSView _rtsView;
    public Unit unitData;
    public GameObject _selectedGameObject;
    private Vector3 movePos;
    private bool currentlyMoving = false;
    public bool isIdle = false;
    private NavMeshAgent _navMeshAgent;
    public float currentIdleTime=0;
    private Transform oldTransform;
    void Awake()
    {
        _selectedGameObject = transform.Find("Selected").gameObject;
        _rtsView = transform.GetComponent<RTSView>();
        SetSelectedVisible(false);
        movePos = transform.position;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        oldTransform = transform;
    }

    private void Update()
    {
        if (!isIdle)
        {
            if (oldTransform.position == transform.position)
            {
                _rtsView.DisableView();
                currentlyMoving = false;
                StayIdle();
            }
            else
            {
                isIdle = false;
                currentIdleTime = 0;
                oldTransform.position = transform.position;
                currentlyMoving = true;
            }
            
        }
    
        if (currentlyMoving)
        {
            _navMeshAgent.destination = movePos;
             _rtsView.EnableView();
            // var direction = movePos - transform.position;
            // Quaternion rotation = Quaternion.LookRotation(-direction, Vector3.up);
            // transform.rotation = rotation;
            // transform.position = Vector3.MoveTowards(transform.position, movePos,unitData.movementSpeed*Time.deltaTime);
        }
    }

    public void SetSelectedVisible(bool visible)
    {
        _selectedGameObject.SetActive(visible);
    }

    public void MoveToPosition(Vector3 moveToPos)
    {
        if (moveToPos == movePos)
            return;
        movePos = moveToPos;
        _navMeshAgent.SetDestination(movePos);
        //_rtsView.SendMoveToPos(movePos);
        isIdle = false;
    }
    private void StayIdle()
    {
        currentIdleTime += Time.deltaTime;
        if (currentIdleTime >= 1f)
        {
            isIdle = true;
        }
    }

    public Transform GetUnit()
    {
        return transform;
    }
}