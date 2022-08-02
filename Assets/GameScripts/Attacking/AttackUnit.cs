using System;
using System.Collections;
using UnityEngine;

public class AttackUnit : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    private UnitSelected unitControlls;
    public Attack state;
    public Attack movement;
    private Vector3 movepos;
    public GameObject target;
    public ResourcesUI resourceUi;
    public float maxRange;
    public bool isAllowedtoAttack;
    public bool currentlyCharing;

    public enum Attack
    {
        Idle,
        MovingToEnemy,
        Fighting,
        Movement,
    }

    private void Awake()
    {
        unitControlls = gameObject.GetComponent<UnitSelected>();
        state = Attack.Idle;
        resourceUi = GameObject.Find("GameManager").GetComponent<ResourcesUI>();
    }

    private void Update()
    {
        try
        {
            switch (state)
            {
                case Attack.Idle:
                    if (movement == Attack.Movement)
                    {
                        target = null;
                        return;
                    }

                    if (target != null)
                    {
                        state = Attack.MovingToEnemy;
                    }

                    break;
                case Attack.MovingToEnemy:
                    if (movement == Attack.Movement)
                    {
                        state = Attack.Idle;
                        target = null;
                        return;
                    }

                    var distance = Mathf.Abs(Vector3.Distance(transform.position, target.transform.position));

                    if (distance > maxRange)
                    {
                        MoveToEnemy(
                            target.transform,
                            () =>
                            {
                                state = Attack.MovingToEnemy;
                            });
                    } else
                    {
                        MoveToEnemy(
                            transform,
                            () =>
                            {
                                state = Attack.Fighting;
                            });
                    }


                    break;
                case Attack.Fighting:
                    if (movement == Attack.Movement)
                    {
                        target = null;
                        return;
                    }

                    var fightdistance = Mathf.Abs(Vector3.Distance(transform.position, target.transform.position));

                    if (fightdistance > maxRange)
                    {
                        state = Attack.MovingToEnemy;
                    } else
                    {
                        if (target != null)
                            AttackTarget();
                        else
                            target = null;
                    }
                    
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            target = null;
            state = Attack.Idle;
            throw;
        }
    }

    private void StartFightingEffect()
    {
        particles.Play();
    }

    private void MoveToEnemy(Transform pos, Action onArrival)
    {
        movepos = pos.position;
        unitControlls.MoveToPosition(movepos);
        onArrival.Invoke();
    }

    private void AttackTarget()
    {
        if (!isAllowedtoAttack && !currentlyCharing)
        {
            StartCoroutine(AttackCooldown());
            StartFightingEffect();
            return;
        }

        if (isAllowedtoAttack)
        {
            ShootProjectile();
        }
    }

    private IEnumerator AttackCooldown()
    {
        currentlyCharing = true;
        yield return new WaitForSeconds(2);
        isAllowedtoAttack = true;
    }

    public void ShootProjectile()
    {
        if (Client.serverlist.ServerlistDictionary[Client.myCurrentServer]
                .PlayerDictionary[Client.otherID].UnitDictionary[target.GetComponent<RTSView>().unit_id].current_hp <=
            0)
            return;

        ClientMessages.ProjectileSpawned(
            Client.myCurrentServer,
            transform.GetComponent<RTSView>().unit_id,
            target.GetComponent<RTSView>().unit_id);
        GameObject arrow = ObjectSpawner.SpawnObject("Arrow", transform.position + transform.forward, new Quaternion());
        arrow.GetComponent<Arrow>().ShootArrowOn(target, unitControlls.unitData.damage);
        arrow.GetComponent<Arrow>().fromEnemy = false;
        currentlyCharing = false;
        isAllowedtoAttack = false;
    }
}
