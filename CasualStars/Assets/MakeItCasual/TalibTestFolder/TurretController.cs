using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;

public class TurretController : TurretNetworkBehavior
{
    public GameObject bulletPrefab;

    public Transform targetPlayer;
    public Transform fireTransform;
    public Transform canonHandle;

    public Vector3 maxRot;
    public Vector3 minRot;

    public float bulletSpeed = 10f;
    public float fireDelay;
    private float time;
    private int bulletRate = 1;

    public bool debug;
    public Text debugText;

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

        rotation1 = canonHandle.eulerAngles;

        canonHandle.LookAt(targetPlayer);

        rotation2 = canonHandle.eulerAngles;

        if (rotation2.x > maxRot.x || rotation2.x < minRot.x)
        {
           canonHandle.rotation = Quaternion.Euler(new Vector3(rotation1.x, rotation1.y, rotation1.z));
        }
        else
        {
           canonHandle.rotation = Quaternion.Euler(new Vector3(rotation2.x, rotation1.y, rotation1.z));
        }

        if(debug)
        debugText.text = rotation2.ToString();

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
        BulletNetworkBehavior shootBullet = NetworkManager.Instance.InstantiateBulletNetwork(0, fireTransform.position, fireTransform.rotation);
        shootBullet.gameObject.GetComponent<Rigidbody>().velocity = bulletSpeed * fireTransform.forward;
    }
}
