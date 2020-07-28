using UnityEngine;

public class DestoryOnColl : MonoBehaviour
{
	GameObject lastHit;
	public string otherTag1;
	public string otherTag2;
	public SceneManagement sm;
	public bool explosion;
	public GameObject explosionBig;
	public float TTED = 10f;

	// Start is called before the first frame update
	private void OnCollisionEnter(Collision collision)
	{
		sm.LoadLostGame();
	}
}
