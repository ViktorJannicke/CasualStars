using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class DestoryOnColl : MonoBehaviour
{
	GameObject lastHit;
	public string otherTag;
	public NGameManager manager;
	public bool explosion;
	public GameObject explosionBig;
	public float TTED = 10f;

	public bool scaleWithDifficulty;

	private void Start()
	{
		if(scaleWithDifficulty)
		{
			Vector3 scale = transform.localScale;
			scale.x *= (MasterManager.mm.difficulty + 1);
			scale.z *= (MasterManager.mm.difficulty + 1);
			transform.localScale = scale;
		}
	}

	// Start is called before the first frame update
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag(otherTag) && lastHit != collision.gameObject)
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
	}
}
