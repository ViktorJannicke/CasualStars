using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	public int health = 50;
	GameObject lastHit;

	public bool split;
	public Obstacle obstacle;

	public GameObject explosionSmall;
	public GameObject explosionBig;
	public float TTED = 10f;

	// Start is called before the first frame update
	private void OnCollisionEnter(Collision collision)
    {
		if (collision.gameObject.CompareTag("Bullet") && lastHit != collision.gameObject)
		{
			lastHit = collision.gameObject;
			health -= collision.gameObject.GetComponent<Bullet>().bulletDamage;
			
			if (health <= 0)
			{
				GameObject explosion = Instantiate(explosionBig, transform.position, transform.rotation);
				explosion.transform.parent = null;
				Destroy(explosion, TTED);

				if (split || obstacle != null)
				obstacle.Kill();
				else
				Destroy(gameObject);
			}
			else
			{
				GameObject explosion = Instantiate(explosionSmall, collision.gameObject.transform);
				explosion.transform.parent = null;
				Destroy(explosion, TTED);
			}

			Destroy(collision.gameObject);
		}
    }
}
