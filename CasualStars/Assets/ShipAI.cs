using UnityEngine;
using UnityEngine.AI;

public class ShipAI : MonoBehaviour
{
    public Movement movement;

    public GameObject target;

    private void Start()
    {
        movement.moveTo(transform.position + new Vector3(1, 0, 0));
    }

    private void Update()
    {
        if (movement.isStopped)
        {
            if (target == null)
            {
                movement.move();
            }
            else
            {
                movement.moveTo(target.transform.position);
            }
        }
    }

    public float distanceToTarget
    {
        get { return Vector3.Distance(target.transform.position, transform.position); }
    }
}

