using UnityEngine;
using UnityEngine.UI;

public class TurretController : MonoBehaviour
{
    public GameObject shootBullet;
    public GameObject bulletPrefab;
    public LaserBeam laser;

    public Transform targetPlayer;
    public Transform fireTransform;

    public float fireDelay;
    public float time = 0;
    private int bulletRate = 1;

    public bool attack = false;

    private void Update()
    {
        if (shootBullet == null)
        {
            laser.DisableLaser();
        }

        if (attack && shootBullet == null)
        {
            if (targetPlayer != null && time == 0)
            {
                for (int x = 0; x < bulletRate; x++)
                {
                    Fire();
                }
                time = fireDelay;
                attack = false;
            }
            else
            {
                float t = Time.deltaTime;

                if (time >= 0 && time - t < 0)
                {
                    time = 0;
                }
                else
                {
                    time -= t;
                }
            }
        }
        else
        {
            float t = Time.deltaTime;

            if (time >= 0 && time - t < 0)
            {
                time = 0;
            }
            else
            {
                time -= t;
            }
        }
    }

    private void Fire()
    {
        shootBullet = Instantiate(bulletPrefab, fireTransform);
        shootBullet.transform.parent = null;
        Bullet b = shootBullet.GetComponent<Bullet>();
        b.targetPlayer = targetPlayer;
        laser.EnableLaser();
        laser.targetPoint = shootBullet.transform;
        b.transform.position = targetPlayer.position;

        //shootBullet.transform.position = targetPlayer.position;
        //shootBullet.GetComponent<Rigidbody>().velocity = bulletSpeed * fireTransform.forward;// + (targetPlayer.position.y > fireTransform.position.y ? Vector3.up : Vector3.down);// + (targetPlayer.position.x > fireTransform.position.x ? Vector3.right : Vector3.left);
    }

    public void SetTargetPlayer(Transform _position, bool _attack)
    {
            targetPlayer = _position;
            attack = _attack;
    }
}