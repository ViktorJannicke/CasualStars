using UnityEngine;
using UnityEngine.AI;

public class Obstacle : MonoBehaviour
{
    public NavMeshAgent agent;
    public Movement movement;

    private void Start()
    {
        movement.moveTo(transform.position + new Vector3(1,0,0));
    }

    private void Update()
    {
        if (movement.isStopped)
        {
            movement.move();
        }
    }
}

