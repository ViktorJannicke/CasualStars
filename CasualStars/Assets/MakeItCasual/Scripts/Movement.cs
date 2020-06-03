using UnityEngine;
using UnityEngine.AI;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;

public class Movement : MovementBehavior
{
    public NavMeshAgent SpaceShip;
    public GameObject cameraControllerObject;

    bool enable = true;
    void Update()
    {
        if (enable && networkObject != null && networkObject.myPlayerID == networkObject.Networker.Me.NetworkId)
        {
            cameraControllerObject.SetActive(true);

            enable = false;
        }

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
