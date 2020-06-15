using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    public NavMeshAgent SpaceShip;
    public GameObject cameraControllerObject;

    public void hyperdrive()
    {
        Vector3 touchposition = NGameManager.manager.getNextPosition();

        SpaceShip.SetDestination(touchposition);
        transform.position = touchposition;
    }

    public void hyperdriveTo(Vector3 touchposition)
    {

        SpaceShip.SetDestination(touchposition);
        transform.position = touchposition;
    }

    public void move()
    {
        Vector3 touchposition = NGameManager.manager.getNextPosition();
        SpaceShip.SetDestination(touchposition);
    }

    public void moveTo(Vector3 touchposition)
    {
        SpaceShip.SetDestination(touchposition);
    }

    public void stop()
    {
        SpaceShip.isStopped = true;
    }

    public bool isStopped { get { return SpaceShip.velocity == Vector3.zero; } }
}
