using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    public float speed = 0.1f;
    public bool move;
    public Transform target;
    public Vector3 targetPos;

    public bool checkX;
    public bool checkY;
    public bool checkZ;

    private void Start()
    {
        targetPos = target.position;
    }

    private void Update()
    {
        if (move)
        {
            Vector3 newPos = Vector3.zero;
            if (checkX && transform.position.x < targetPos.x && (targetPos.x - transform.position.x > speed))
            {
                newPos.x += speed;
            }
            else if (checkX && transform.position.x > targetPos.x && (transform.position.x - targetPos.x > speed))
            {
                newPos.x -= speed;
            }

            if (checkY && transform.position.y < targetPos.y && (targetPos.y - transform.position.y > speed))
            {
                newPos.y += speed;
            }
            else if (checkY && transform.position.y > targetPos.y && (transform.position.y - targetPos.y > speed))
            {
                newPos.y -= speed;
            }

            if (checkZ && transform.position.z < targetPos.z && (targetPos.z - transform.position.z > speed))
            {
                newPos.z += speed;
            }
            else if (checkZ && transform.position.z > targetPos.z && (transform.position.z - targetPos.z > speed))
            {
                newPos.z -= speed;
            }

            if(Vector3.Distance(new Vector3(checkX ? transform.position.x : 0, checkY ? transform.position.y : 0, checkZ ? transform.position.z : 0), new Vector3(checkX ? targetPos.x : 0, checkY ? targetPos.y : 0, checkZ ? targetPos.z : 0)) <= speed)
            {
                move = false;
            }

            transform.position += newPos;
        }
    }

    public void stop()
    {
        targetPos = transform.position;
    }

    public bool isStopped { get { return !move; } }
}
