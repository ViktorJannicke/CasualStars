using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
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

	public DoInput input;

	public void triggerSetHyperdrive(bool val)
	{
		input.setHyperdrive(val);
	}

	private void Start()
	{
		spawnPlayer();

		//Asteroidsspawn();
	}

	public void spawnPlayer()
	{
		Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), 3, Random.Range(-size.z / 2, size.z / 2));
		
		while(Physics.CheckSphere(pos, spawnRadius))
		{
			pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), 3, Random.Range(-size.z / 2, size.z / 2));
		}

		BasicBehavior bh = NetworkManager.Instance.InstantiateBasic(0, pos, Quaternion.identity);

		input = bh.gameObject.GetComponent<DoInput>();
		input.enabled = true;
	}

	public void Asteroidsspawn()
	{
		Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));

		while(Physics.CheckSphere(pos, spawnRadius))
		{
			pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
		}
		Instantiate(Asteroidprefab, pos, Quaternion.identity);

	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1, 0, 0, 0.5f);

		Gizmos.DrawCube(transform.localPosition + center, size);
	}








}