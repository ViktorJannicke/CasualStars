using UnityEngine;
using UnityEngine.AI;
using BeardedManStudios.Forge.Networking.Generated;

public class Movement : MovementBehavior
{
    public NavMeshAgent SpaceShip;

    void Update()
    {
        if (networkObject == null)
            return;

        if (!networkObject.IsOwner)
        {
            transform.position = networkObject.position;
            transform.rotation = networkObject.rotation;
        }
        else
        {
            networkObject.position = transform.position;
            networkObject.rotation = transform.rotation;
        }
    }

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
