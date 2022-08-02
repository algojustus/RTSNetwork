using UnityEngine;
using UnityEngine.AI;

public class Arrow : MonoBehaviour
{
    public NavMeshAgent navmeshagent;
    public Transform targetPos;
    public UnitController uiController;
    public bool fromEnemy;
    public int damage;

    private void Awake()
    {
        navmeshagent = transform.GetComponent<NavMeshAgent>();
        uiController = GameObject.Find("GameManager").GetComponent<UnitController>();
    }

    void Update()
    {
        navmeshagent.destination = targetPos.position;
    }

    public void ShootArrowOn(GameObject target, int _damage)
    {
        damage = _damage;
        targetPos = target.transform;
        navmeshagent.SetDestination(targetPos.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (fromEnemy)
        {
            if (!other.transform.CompareTag("Player1"))
                return;
        } else
        {
            if (!other.transform.CompareTag("Player2"))
                return;
        }

        int hitUnitID = other.GetComponent<RTSView>().unit_id;
        int leftoverhp;
        if (fromEnemy)
            leftoverhp = Client.serverlist.ServerlistDictionary[Client.myCurrentServer]
                .PlayerDictionary[Client.clientID].UnitDictionary[hitUnitID].current_hp -= damage;
        else
            leftoverhp = Client.serverlist.ServerlistDictionary[Client.myCurrentServer]
                .PlayerDictionary[Client.otherID].UnitDictionary[hitUnitID].current_hp -= damage;
        if (leftoverhp <= 0)
        {
            uiController.DeactivateAllUnitsBeforeNewSelect();
            Destroy(targetPos.gameObject);
        }

        Destroy(gameObject);
    }
}
