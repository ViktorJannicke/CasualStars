using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    public List<TurretController> turrets = new List<TurretController>();

    public void setTarget(Transform targetTransform)
    {
                foreach (TurretController turret in turrets)
                {
                    turret.SetTargetPlayer(targetTransform, true);
                }
    }
}
