using UnityEngine;
using UnityEngine.AI;


public class UnitSelected : MonoBehaviour
{
    private RTSView _rtsView;
    public Unit unitData;
    public GameObject _selectedGameObject;
    private NavMeshAgent _navMeshAgent;
    private Transform oldTransform;
    private Vector3 movePos;
    private bool currentlyMoving = false;
    public bool isIdle = false;
    public float currentIdleTime=0;
    
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
        _rtsView.SendMoveToPos(movePos);
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