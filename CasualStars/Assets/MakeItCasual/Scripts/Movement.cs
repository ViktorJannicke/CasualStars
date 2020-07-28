using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 51f; //vergessen im gamemanager einzustellen//
    public bool move;
    public bool isSatelite;
    public Transform target;
    public Vector3 targetPos;

    public bool destoryOnTargetReached;

    private void Update()
    {
        if (move)
        {
            if (target != null)
            {
                    targetPos = target.position;
            }

            var heading = targetPos - transform.position;
            var distance = heading.magnitude;
            Vector3 direction = heading / distance;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            heading = targetPos - transform.position;
            distance = heading.magnitude;
            if (distance < 1)
            {
                move = false;

                if(destoryOnTargetReached)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void stop()
    {
        targetPos = transform.position;
    }

    public bool isStopped { get { return !move; } }
}
