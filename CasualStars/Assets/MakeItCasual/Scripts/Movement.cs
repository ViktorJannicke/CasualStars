using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    public NavMeshAgent SpaceShip;
    public GameObject cameraControllerObject;

    public void hyperdrive(Vector3 touchposition)
    {
        SpaceShip.SetDestination(touchposition);
        transform.position = touchposition;
    }
    public void move(Vector3 touchposition)
    {
        SpaceShip.SetDestination(touchposition);
    }
}
