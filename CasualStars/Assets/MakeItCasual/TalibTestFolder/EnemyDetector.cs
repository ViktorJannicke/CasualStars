using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : DummyBehavior
{
    public List<TurretController> turrets = new List<TurretController>();
    List<GameObject> enemys = new List<GameObject>();

    private void Update()
    {
        if (networkObject == null)
            return;

        if (networkObject.IsOwner)
        {
            foreach (TurretController turret in turrets)
            {
                float closestDistance = 10000f;
                GameObject closestEnemy = null;

                foreach (GameObject enemy in enemys)
                {
                    float distance = Vector3.Distance(gameObject.transform.position, enemy.transform.position);

                    if (closestDistance > distance)
                    {
                        closestDistance = distance;
                        closestEnemy = enemy;
                    }
                }
                if(closestDistance != 10000f)
                turret.TriggerSetTargetPlayer(closestEnemy.transform.position, true);
                else
                turret.TriggerSetTargetPlayer(Vector3.zero, false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject != gameObject)
        {
            if (!enemys.Contains(other.gameObject))
            {
                enemys.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject != gameObject)
        {
            if (enemys.Contains(other.gameObject))
            {
                enemys.Remove(other.gameObject);
            }
        }
    }
}
