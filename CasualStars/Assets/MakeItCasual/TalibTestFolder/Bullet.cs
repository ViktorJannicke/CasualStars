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
            transform.position = networkObject.position;
            transform.rotation = networkObject.rotation;
        }
        else
        {
            networkObject.position = transform.position;
            networkObject.rotation = transform.rotation;
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
