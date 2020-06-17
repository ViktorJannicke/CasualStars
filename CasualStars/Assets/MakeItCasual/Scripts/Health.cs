using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	public int health = 50;
	GameObject lastHit;

	public bool split;
	public Obstacle obstacle;

	// Start is called before the first frame update
	private void OnCollisionEnter(Collision collision)
    {
		if (collision.gameObject.CompareTag("Bullet") && lastHit != collision.gameObject)
		{

			lastHit = collision.gameObject;
			health -= lastHit.GetComponent<Bullet>().bulletDamage;
			Destroy(lastHit.GetComponent<Bullet>().gameObject);
			
			if (health <= 0)
			{
				if (split || obstacle != null)
				obstacle.Kill();
				else
				Destroy(gameObject);
			}
		}
    }
}
