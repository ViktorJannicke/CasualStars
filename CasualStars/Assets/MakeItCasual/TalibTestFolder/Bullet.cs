using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;

public class Bullet : BulletNetworkBehavior
{
    public float bulletDamage = 100f;

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
    }
}
