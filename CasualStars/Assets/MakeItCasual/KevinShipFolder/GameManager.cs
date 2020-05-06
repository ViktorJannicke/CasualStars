using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameObject Asteroidprefab;
	public Vector3 center;
	public Vector3 size;
	public bool stopSpawning = false;
	public int spawnTime;
	public int spawnDelay;
	public int spawnRadius;

	private void Start()
	{
		Asteroidsspawn();
	}

	public void Asteroidsspawn()
	{
		Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));

		if(!Physics.CheckSphere(pos, spawnRadius))
		Instantiate(Asteroidprefab, pos, Quaternion.identity);

	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1, 0, 0, 0.5f);

		Gizmos.DrawCube(transform.localPosition + center, size);
	}








}