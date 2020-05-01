using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestShooting : MonoBehaviour
{
    public Transform target;

    public GameObject bullet;
    public Transform fireTransform;

    public float bulletSpeed = 10f;
    public float bulletDamage = 100f;
    public float shootCooldown = 2f;
    [HideInInspector] public float cooldown;

    public Rigidbody rb;
    public SphereCollider shootRadius;
    public Transform turret;

    bool enemyInRadius = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        shootRadius = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        cooldown -= Time.deltaTime;
        if(enemyInRadius == true)
        {
            if(cooldown <= 0f)
            {
                Fire();
                cooldown = shootCooldown;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        turret.transform.LookAt(target);
        fireTransform.transform.LookAt(target);
        Debug.Log("Enemy in Radius");
        enemyInRadius = true;
    }

    private void Fire()
    {
        GameObject shootBullet = Instantiate(bullet, fireTransform.position, fireTransform.rotation);
        
        shootBullet.GetComponent<Rigidbody>().velocity = bulletSpeed * fireTransform.forward;
    }

    /*private void ClickAimShoot()
    {

    }*/
}
