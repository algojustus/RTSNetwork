using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelected : MonoBehaviour
{
    private GameObject _selectedGameObject;
    private Vector3 movePos;
    [SerializeField] private float movementSpeed = 30;
    private bool currentlyMoving = false;
    private RTSView _rtsView;
    void Awake()
    {
        
        _selectedGameObject = transform.Find("Selected").gameObject;
        _rtsView = transform.GetComponent<RTSView>();
        SetSelectedVisible(false);
        movePos = transform.position;
    }

    private void Update()
    {
        if (currentlyMoving)
        {
            _rtsView.EnableView();
            var direction = movePos - transform.position;
            Quaternion rotation = Quaternion.LookRotation(-direction, Vector3.up);
            transform.rotation = rotation;
            transform.position = Vector3.MoveTowards(transform.position, movePos,movementSpeed*Time.deltaTime);
        }

        if (transform.position == movePos)
        {
            _rtsView.DisableView();
            currentlyMoving = false;
        }
    }

    public void SetSelectedVisible(bool visible)
    {
        _selectedGameObject.SetActive(visible);
    }

    public void MoveToPosition(Vector3 moveToPos)
    {
        movePos = moveToPos;
        _rtsView.SendMoveToPos(movePos);
        currentlyMoving = true;
    }
}