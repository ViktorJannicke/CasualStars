using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{

	public float  speedModifier;

	public NavMeshAgent SpaceShip;


	public void move(Vector3 touchposition)
	{
		SpaceShip.SetDestination(touchposition);
		Debug.Log("Test? Hörst du mich Welt, ich bin ein einsamer Mensch Rufst du mich überhaupt auf!!!!!!??????");
	}
}
