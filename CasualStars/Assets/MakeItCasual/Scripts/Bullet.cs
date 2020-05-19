using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using System.Threading;

public class Bullet : BulletNetworkBehavior
{
    public int bulletDamage = 0;
    public float timer;
    public float timeTillDeath;
    private void Update()
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

        if(timer >= timeTillDeath)
        {
            networkObject.Destroy();
        }
        else if (timer < timeTillDeath)
        {
            timer += Time.deltaTime;
        }
    }
}
