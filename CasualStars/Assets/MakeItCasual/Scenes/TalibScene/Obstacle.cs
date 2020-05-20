using UnityEngine;
using UnityEngine.AI;

public class Obstacle : MonoBehaviour
{
    public NavMeshAgent agent;
    public Movement movement;

    private void Start()
    {
        movement.move(transform.position + new Vector3(1,0,0));
    }

    private void Update()
    {
        if (Vector3.Distance(agent.destination, transform.position) <= 1)
        {
            movement.move(NGameManager.manager.getNextPosition());
        }
    }
}
