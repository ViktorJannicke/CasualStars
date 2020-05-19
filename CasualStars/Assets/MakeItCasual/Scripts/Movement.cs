using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;

public class Movement : MovementBehavior
{

    // Update is called once per frame
    void Update()
    {
        if(!networkObject.IsOwner)
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
}
