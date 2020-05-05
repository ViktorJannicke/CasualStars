using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeardedManStudios.Forge.Networking.Generated;

public class FireController : FireBulletBehavior
{
    public GameObject bulletPrefab;
    public Text kdfkf;

    public Transform targetPlayer;
    public Transform fireTransform;

    public Vector3 maxRot;
    public Vector3 minRot;

    public float bulletSpeed = 10f;
    private int bulletRate = 1;
    public float fireDelay;
    public float time;

    private void Start()
    {
        time = 0f;
    }

    private void FixedUpdate()
    {
        Vector3 rotation1 = transform.eulerAngles;

        transform.LookAt(targetPlayer);

        Vector3 rotation2 = transform.eulerAngles;

        if (rotation2.y > maxRot.y || rotation2.y < minRot.y)
        {
            transform.rotation = Quaternion.Euler(new Vector3(rotation1.x, rotation1.y, rotation1.z));
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(rotation1.x, rotation2.y, rotation1.z));
        }

        kdfkf.text = rotation2.ToString();

        if (targetPlayer != null && time > fireDelay)
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

    private void Fire()
    {
        GameObject shootBullet  = Instantiate(bulletPrefab, fireTransform.position, fireTransform.rotation);
        shootBullet.GetComponent<Rigidbody>().velocity = bulletSpeed * fireTransform.forward;
    }
}
