using UnityEngine;

public class DestoryOnColl : MonoBehaviour
{
	GameObject lastHit;
	public string otherTag1;
	public string otherTag2;
	public NGameManager manager;
	public bool explosion;
	public GameObject explosionBig;
	public float TTED = 10f;

	// Start is called before the first frame update
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag(otherTag1) && lastHit != collision.gameObject)
		{
			lastHit = collision.gameObject;

			if (explosion)
			{
				GameObject explosion = Instantiate(explosionBig, transform.position, transform.rotation);
				explosion.transform.parent = null;
				Destroy(explosion, TTED);
			}

			int bonus = collision.gameObject.GetComponent<Obstacle>().ScoreBonus;
			
			if (manager.Score - bonus <= 0)
				manager.Score = 0;
			else
				manager.Score -= bonus;


			Destroy(collision.gameObject);
		}
		if (collision.gameObject.CompareTag(otherTag2) && lastHit != collision.gameObject)
		{
			lastHit = collision.gameObject;

			Destroy(collision.gameObject);
		}
	}
}
