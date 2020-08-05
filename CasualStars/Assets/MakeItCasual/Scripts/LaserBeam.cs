using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
	public GameObject laser;
	public LineRenderer laserRenderer;
	public Transform targetPoint;

	bool disableDelayed;
	float timer;
	float timerMax = 0.5f;

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

		if(disableDelayed && timerMax <= timer)
        {
			disableDelayed = false;
			timer = 0;

			if (laser.activeSelf)
			{
				laser.SetActive(false);
			}
		}
		else if (timer < timerMax)
		{
			timer += Time.deltaTime;
		}
	}

	public void DisableLaser()
	{
		disableDelayed = true;
	}























}
