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
    public int shootedBullets;

    public Rigidbody rb;
    
    bool enemyInRadius = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        shootedBullets = 0;
    }

    private void Update()
    {
        if (enemyInRadius)
        {
            Fire();
        }
        if(shootedBullets == bulletRate)
        {
           
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        enemyInRadius = true;
        Debug.Log("Enemy in Radius");
        turret.transform.LookAt(target);
        fireTransform.transform.LookAt(target);
    }

    private void OnTriggerExit(Collider other)
    {
        enemyInRadius = false;
        Debug.Log("Enemy not in Radius");
    }

    private void Fire()
    {
        GameObject shootBullet = Instantiate(bullet, fireTransform.position, fireTransform.rotation);
        shootedBullets += 1;

        shootBullet.GetComponent<Rigidbody>().velocity = bulletSpeed * fireTransform.forward;
    }

    /*private void ClickAimShoot()
    {

    }*/
}
