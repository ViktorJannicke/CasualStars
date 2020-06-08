using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using System.Threading;
using UnityEngine.InputSystem.LowLevel;

public class Bullet : BulletNetworkBehavior
{

    public int bulletDamage = 0;
    public float timer;
    public float timeTillDeath;
    public float spawnDelay = 5;

    bool disabled = true;
    private void Update()
    {
        if (disabled)
        {
            if (timer >= spawnDelay)
            {
                timer = 0;
                disabled = false;
            }
            else if (timer < spawnDelay)
            {
                timer += Time.deltaTime;
            }
        }
        else
        {
            if (networkObject == null)
                return;

            if (!networkObject.IsOwner)
            {
                transform.position = networkObject.bulletPosition;
                transform.rotation = networkObject.bulletRotation;
            }
            else
            {
                networkObject.bulletPosition = transform.position;
                networkObject.bulletRotation = transform.rotation;
            }

            if (timer >= timeTillDeath)
            {
                networkObject.Destroy();
            }
            else if (timer < timeTillDeath)
            {
                timer += Time.deltaTime;
            }
        }
    }
}
