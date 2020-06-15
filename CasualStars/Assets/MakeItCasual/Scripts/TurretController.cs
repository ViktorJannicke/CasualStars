using UnityEngine;
using UnityEngine.UI;

public class TurretController : MonoBehaviour
{
    public GameObject bulletPrefab;

    public Transform targetPlayer;
    public Transform fireTransform;
    public Transform canonHandle;

    public Vector3 maxRot;
    public Vector3 minRot;
    public Vector3 baseRot;

    public bool enableXRotation;
    public bool enableYRotation;
    public bool enableZRotation;

    public float bulletSpeed = 10f;
    public float fireDelay;
    private float time;
    private int bulletRate = 1;

    public bool attack = false;
    public bool debug;
    public Text debugText;

    private void FixedUpdate()
    {
            if (attack)
            {
                Vector3 rotation1 = transform.eulerAngles;

                transform.LookAt(targetPlayer);

                Vector3 rotation2 = transform.eulerAngles;

                if (!enableYRotation && (rotation2.y > maxRot.y || rotation2.y < minRot.y))
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

                if (!enableXRotation && (rotation2.x > maxRot.x || rotation2.x < minRot.x))
                {
                    canonHandle.rotation = Quaternion.Euler(new Vector3(rotation1.x, rotation1.y, rotation1.z));
                }
                else
                {
                    canonHandle.rotation = Quaternion.Euler(new Vector3(rotation2.x, rotation1.y, rotation1.z));
                }

                rotation1 = canonHandle.eulerAngles;

                canonHandle.LookAt(targetPlayer);

                rotation2 = canonHandle.eulerAngles;

                if (!enableZRotation && (rotation2.z > maxRot.z || rotation2.z < minRot.z))
                {
                    canonHandle.rotation = Quaternion.Euler(new Vector3(rotation1.x, rotation1.y, rotation1.z));
                }
                else
                {
                    canonHandle.rotation = Quaternion.Euler(new Vector3(rotation2.x, rotation1.y, rotation2.z));
                }

                if (debug)
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
            else
            {
                transform.localRotation = Quaternion.Euler(new Vector3(180, baseRot.y, 0));
                canonHandle.localRotation = Quaternion.Euler(new Vector3(baseRot.x,0,baseRot.z));
            }
    }

    private void Fire()
    {
        GameObject shootBullet = Instantiate(bulletPrefab, fireTransform);
        shootBullet.GetComponent<Rigidbody>().velocity = bulletSpeed * fireTransform.forward;
        shootBullet.transform.parent = null;
    }

    public void SetTargetPlayer(Vector3 _position, bool _attack)
    {
            targetPlayer.position = _position;
            attack = _attack;
    }
}