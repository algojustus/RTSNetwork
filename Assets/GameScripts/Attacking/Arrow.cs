using System;
using UnityEngine;
using UnityEngine.AI;

public class Arrow : MonoBehaviour
{
    public NavMeshAgent navmeshagent;
    public Transform targetPos;
    private void Awake()
    {
        navmeshagent = transform.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        navmeshagent.destination = targetPos.position;
    }

    public void ShootArrowOn(GameObject target)
    {
        targetPos = target.transform;
        navmeshagent.SetDestination(targetPos.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Player2"))
            return;
        
        Destroy(gameObject);
    }
}
