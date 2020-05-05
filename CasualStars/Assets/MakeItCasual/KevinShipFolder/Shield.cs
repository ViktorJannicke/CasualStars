using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;

public class Shield : NetworkHealthBehavior
{

	public int health = 50;
	GameObject lastHit;

    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {
		if (collision.gameObject.CompareTag("Bullet") && lastHit != collision.gameObject)
		{

			lastHit = collision.gameObject;
			health -= lastHit.GetComponent<Bullet>().bulletDamage;
			lastHit.GetComponent<Bullet>().networkObject.Destroy();

			if(health <= 0)
			{
				networkObject.Destroy();
			}

		}
    }

    // Update is called once per frame
    void Update()
    {

		// Unity's Update() running, before this object is instantiated
		// on the network is **very** rare, but better be safe 100%
		if (networkObject == null)
			return;

		// If we are not the owner of this network object then we should
		// move this cube to the position/rotation dictated by the owner
		if (!networkObject.IsOwner)
		{

			health = networkObject.Health;
			return;

		}
		else
		{

			networkObject.Health = health;
			
		}
	}
}
