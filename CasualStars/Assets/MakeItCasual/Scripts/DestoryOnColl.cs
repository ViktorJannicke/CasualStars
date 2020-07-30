using UnityEngine;

public class DestoryOnColl : MonoBehaviour
{
	GameObject lastHit;
	public string otherTag1;
	public string otherTag2;
	public SceneManagement sm;
	public bool explosion;
	public GameObject explosionSmall;
	public GameObject explosionBig;
	public float TTED = 10f;
	public float delay;
	public Vector3 explosionOffset;



	// Start is called before the first frame update
	private void OnCollisionEnter(Collision collision)
	{
		if ((collision.gameObject.CompareTag("AsteroidIn") || collision.gameObject.CompareTag("AsteroidOut")) && lastHit != collision.gameObject)
		{
			lastHit = collision.gameObject;
			destroyEffect();
			sm.LoadLostGame();
		}
	}

	public void destroyEffect()
	{
			GameObject explosion = Instantiate(explosionBig, transform.position + explosionOffset, transform.rotation);
			explosion.transform.parent = null;
			Destroy(explosion, TTED);
	}
}
