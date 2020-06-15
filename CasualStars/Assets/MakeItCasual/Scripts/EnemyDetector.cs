using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public List<TurretController> turrets = new List<TurretController>();
    public List<GameObject> enemys = new List<GameObject>();
    public GameObject forcedTarget;
    public ShipAI ai;

    public bool turretOrAI;

    private void Update()
    {
        if (forcedTarget == null)
        {
            if (turretOrAI) {
                foreach (TurretController turret in turrets)
                {
                    float closestDistance = 10000f;
                    GameObject closestEnemy = null;

                    if (enemys.Count != 0)
                    {
                        for (int i = 0; i < enemys.Count; i++)
                        {
                            GameObject enemy = enemys[i];
                            if (enemy != null)
                            {
                                float distance = Vector3.Distance(gameObject.transform.position, enemy.transform.position);

                                if (closestDistance > distance)
                                {
                                    closestDistance = distance;
                                    closestEnemy = enemy;
                                }

                                if (closestDistance != 10000f && closestEnemy != null)
                                {
                                    turret.SetTargetPlayer(closestEnemy.transform.position, true);
                                }
                                else
                                {
                                    turret.SetTargetPlayer(Vector3.zero, false);
                                }
                            }
                            else
                            {
                                enemys.RemoveAt(i);
                                turret.SetTargetPlayer(Vector3.zero, false);
                            }
                        }
                    }
                    else
                    {
                        turret.SetTargetPlayer(Vector3.zero, false);
                    }
                }
            }
            else
            {
                float closestDistance = 10000f;
                GameObject closestEnemy = null;

                if (enemys.Count != 0)
                {
                    for (int i = 0; i < enemys.Count; i++)
                    {
                        GameObject enemy = enemys[i];
                        if (enemy != null)
                        {
                            float distance = Vector3.Distance(gameObject.transform.position, enemy.transform.position);

                            if (closestDistance > distance)
                            {
                                closestDistance = distance;
                                closestEnemy = enemy;
                            }

                            if (closestDistance != 10000f && closestEnemy != null)
                            {
                                ai.target = closestEnemy;
                            }
                            else
                            {
                                ai.target = null;
                            }
                        }
                        else
                        {
                            enemys.RemoveAt(i);
                            ai.target = null;
                        }
                    }
                }
                else
                {
                    ai.target = null;
                }
            }
        }
        else if (forcedTarget != null)
        {
            if (turretOrAI)
            {
                foreach (TurretController turret in turrets)
                {
                    turret.SetTargetPlayer(forcedTarget.transform.position, true);


                }
            } else
            {
                ai.target = forcedTarget;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!enemys.Contains(other.gameObject))
            {
                enemys.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (enemys.Contains(other.gameObject))
            {
                enemys.Remove(other.gameObject);
            }
        }
    }
}
