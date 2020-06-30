using UnityEngine;
using UnityEngine.UI;

public class TurretController : MonoBehaviour
{
    public GameObject bulletPrefab;

    public Transform targetPlayer;
    public Transform fireTransform;
    public Transform canonHandle;

    public float bulletSpeed = 10f;
    public float fireDelay;
    private float time = 0;
    private int bulletRate = 1;

    public bool attack = false;
    public bool debug;
    public Text debugText;

    private void Update()
    {
        if (attack)
        {
            if (targetPlayer != null && time == 0)
            {
                for (int x = 0; x < bulletRate; x++)
                {
                    Fire();
                    attack = false;
                }
                time = fireDelay;
            }
            else
            {
                float t = Time.deltaTime;

                if (time - t <= 0)
                {
                    time -= 0;
                }
                else
                {
                    time -= t;
                }
            }
        }
    }

    private void Fire()
    {
        fireTransform.LookAt(targetPlayer);
        GameObject shootBullet = Instantiate(bulletPrefab, fireTransform);
        shootBullet.transform.parent = null;
        Bullet b = shootBullet.GetComponent<Bullet>();
        b.targetPlayer = targetPlayer;
        b.move = true;
        //shootBullet.transform.position = targetPlayer.position;
        //shootBullet.GetComponent<Rigidbody>().velocity = bulletSpeed * fireTransform.forward;// + (targetPlayer.position.y > fireTransform.position.y ? Vector3.up : Vector3.down);// + (targetPlayer.position.x > fireTransform.position.x ? Vector3.right : Vector3.left);
    }

    public void SetTargetPlayer(Transform _position, bool _attack)
    {
            targetPlayer = _position;
            attack = _attack;
    }
}