using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;

public class FireController : FireBulletBehavior
{
    public GameObject bulletPrefab;

    public Transform targetPlayer;
    public Transform shipTurret;
    public Transform fireTransform;

    public float bulletSpeed = 10f;
    private int bulletRate = 10;
    public float fireDelay;
    private float delay = 1f;
    public float time;

    public Rigidbody shipRb;

    private void Start()
    {
        shipRb = GetComponent<Rigidbody>();
        time = 0f;
    }

    private void Update()
    {
        if(targetPlayer != null && time > fireDelay + delay)
        {
            for (int x = 0; x < bulletRate; x++)
            {
                Fire();
            }
            time = 0;
        }
        else
        {
            time += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enemy in Radius");
        shipTurret.transform.LookAt(targetPlayer);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Enemy not in Radius");
    }

    private void Fire()
    {
        GameObject shootBullet = Instantiate(bulletPrefab, fireTransform.position, fireTransform.rotation);

        shootBullet.GetComponent<Rigidbody>().velocity = bulletSpeed * fireTransform.forward;
    }
}
