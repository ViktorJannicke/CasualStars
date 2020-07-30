using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
	public GameObject laser;
	public LineRenderer laserRenderer;
	public Transform targetPoint;

	public void EnableLaser()
	{
		if (!laser.activeSelf)
		{
			laser.SetActive(true);
		}
	}

	void Update()
	{
		if (targetPoint != null)
		{
			Vector3 heading = targetPoint.position - transform.position;
			//var distance = heading.magnitude;
			//Vector3 direction = heading / distance;
			laserRenderer.SetPositions(new Vector3[] { Vector3.zero, heading });
		}
	}

	public void DisableLaser()
	{
		if (laser.activeSelf)
		{
			laser.SetActive(false);
		}
	}























}
