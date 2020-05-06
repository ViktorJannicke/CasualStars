using UnityEngine;
using UnityEngine.UI;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking;

public class TurretController : TurretNetworkBehavior
{
    public GameObject bulletPrefab;

    public Transform targetPlayer;
    public Transform fireTransform;
    public Transform canonHandle;

    public Vector3 maxRot1;
    public Vector3 maxRot2;
    public Vector3 minRot1;
    public Vector3 minRot2;

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
        if (networkObject == null)
            return;

        if (!networkObject.IsOwner)
        {
            transform.position = networkObject.turretPosition;
            transform.rotation = networkObject.turretRotation;
            canonHandle.localPosition = networkObject.handlePosition;
            canonHandle.localRotation = networkObject.handleRotation;
        }
        else
        {
            if (attack)
            {
                Vector3 rotation1 = transform.eulerAngles;

                transform.LookAt(targetPlayer);

                Vector3 rotation2 = transform.eulerAngles;

                if (!enableYRotation && (rotation2.y > maxRot1.y && rotation2.y > maxRot2.y || rotation2.y < minRot1.y && rotation2.y < minRot2.y))
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

                if (!enableXRotation && (rotation2.x > maxRot1.x && rotation2.x > maxRot2.x || rotation2.x < minRot1.x && rotation2.x < minRot2.x))
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

                if (!enableZRotation && (rotation2.z > maxRot1.z && rotation2.z > maxRot2.z || rotation2.z < minRot1.z && rotation2.z < minRot2.z))
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
                transform.rotation = Quaternion.Euler(Vector3.zero);
                canonHandle.rotation = Quaternion.Euler(Vector3.zero);
            }

            networkObject.turretPosition = transform.position;
            networkObject.turretRotation = transform.rotation;
            networkObject.handlePosition = canonHandle.localPosition;
            networkObject.handleRotation = canonHandle.localRotation;
        }
    }

    private void Fire()
    {
        BulletNetworkBehavior shootBullet = NetworkManager.Instance.InstantiateBulletNetwork(0, fireTransform.position, fireTransform.rotation);
        shootBullet.gameObject.GetComponent<Rigidbody>().velocity = bulletSpeed * fireTransform.forward;
    }

    public override void SetTargetPlayer(RpcArgs args)
    {
        targetPlayer.position = args.GetNext<Vector3>();
        attack = args.GetNext<bool>();
    }

    public void TriggerSetTargetPlayer(Vector3 position, bool attack)
    {
        networkObject.SendRpc(RPC_SET_TARGET_PLAYER,Receivers.Server,position, attack);
    }
}