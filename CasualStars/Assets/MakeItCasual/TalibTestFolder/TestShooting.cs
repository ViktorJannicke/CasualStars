using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestShooting : MonoBehaviour
{
    public GameObject bullet;

    public Transform target;
    public Transform turret;
    public Transform fireTransform;

    public float bulletSpeed = 10f;
    //public float bulletDamage = 100f;
    int bulletRate = 10;
    public float fireDelay;
    private float delay = 1;

    public float time;

    public Rigidbody rb;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        time = 0;
    }

    private void Update()
    {
        if (target != null && time > fireDelay + delay)
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
        turret.transform.LookAt(target);
        //fireTransform.transform.LookAt(target);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Enemy not in Radius");
    }

    private void Fire()
    {
        GameObject shootBullet = Instantiate(bullet, fireTransform.position, fireTransform.rotation);

        shootBullet.GetComponent<Rigidbody>().velocity = bulletSpeed * fireTransform.forward;
    }
}
