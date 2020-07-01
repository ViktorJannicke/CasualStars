using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAsteroid : MonoBehaviour
{
    public NGameManager manager;
    public float ZOffset;

    private void OnTriggerEnter(Collider other)
    {
        manager.spawnMovingAsteroid(transform.position + new Vector3(0,0,ZOffset));
        Destroy(gameObject);
    }
}
