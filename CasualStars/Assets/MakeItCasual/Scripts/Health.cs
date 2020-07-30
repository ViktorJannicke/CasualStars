﻿using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
	public int health = 50;
	GameObject lastHit;

	public Obstacle obstacle;

	public GameObject explosionSmall;
	public GameObject explosionBig;
	public float TTED = 10f;
	public Vector3 explosionOffset;
	public float delay;

	// Start is called before the first frame update
	private void OnCollisionEnter(Collision collision)
    {
		if (collision.gameObject.CompareTag("Bullet") && lastHit != collision.gameObject)
		{
			lastHit = collision.gameObject;
			health -= collision.gameObject.GetComponent<Bullet>().bulletDamage;
			StartCoroutine(destroyEffect(collision));
		}
    }

	IEnumerator destroyEffect(Collision collision)
    {
		yield return new WaitForSeconds(delay);

			if (health <= 0)
			{
				GameObject explosion = Instantiate(explosionBig, transform.position + explosionOffset, transform.rotation);
				explosion.transform.parent = null;
				Destroy(explosion, TTED);

				obstacle.Kill();
			}
			else
			{
				GameObject explosion = Instantiate(explosionSmall, transform.position + explosionOffset, transform.rotation);
				explosion.transform.parent = null;
				Destroy(explosion, TTED);
			}
	}
}
