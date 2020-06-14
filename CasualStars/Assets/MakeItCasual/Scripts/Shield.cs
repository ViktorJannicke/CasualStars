using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
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
			Destroy(lastHit.GetComponent<Bullet>().gameObject);

			if(health <= 0)
			{
				Destroy(gameObject);
			}

		}
    }
}
