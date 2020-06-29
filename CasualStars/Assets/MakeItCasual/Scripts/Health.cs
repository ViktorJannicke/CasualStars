using UnityEngine;

public class Health : MonoBehaviour
{
	public int health = 50;
	GameObject lastHit;

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

				obstacle.Kill();
			}
			else
			{
				GameObject explosion = Instantiate(explosionSmall, collision.contacts[0].point, transform.rotation);
				explosion.transform.parent = null;
				Destroy(explosion, TTED);
			}

			Destroy(collision.gameObject);
		}
    }
}
