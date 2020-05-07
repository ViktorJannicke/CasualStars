using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine.PlayerLoop;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;

public class Movement : BasicBehavior
{
	public NavMeshAgent SpaceShip;

	private void Update ()
	{
		// Unity's Update() running, before this object is instantiated
		// on the network is **very** rare, but better be safe 100%
		if (networkObject == null)
			return;

		// If we are not the owner of this network object then we should
		// move this cube to the position/rotation dictated by the owner
		if (!networkObject.IsOwner)
		{
			transform.position = networkObject.Position;
			transform.rotation = networkObject.Rotation;
			return;
		}
		else
		{
			networkObject.Position = transform.position;
			networkObject.Rotation = transform.rotation;
		}
	}

	public void hyperdrive(Vector3 touchposition)
	{
		networkObject.SendRpc(RPC_HYPERDRIVE, Receivers.Server, touchposition);
	}
	public void move(Vector3 touchposition)
	{
		networkObject.SendRpc(RPC_MOVE_TO, Receivers.Server, touchposition);
	}

	public override void MoveTo(RpcArgs args)
	{
		// RPC calls are not made from the main thread for performance, since we
		// are interacting with Unity engine objects, we will need to make sure
		// to run the logic on the main thread
		MainThreadManager.Run(() =>
		{
			SpaceShip.SetDestination(args.GetNext<Vector3>());
			
		});
	}
	public override void Hyperdrive(RpcArgs args)
	{
		//SpaceShip.SetDestination(args.GetNext<Vector3>());

		Vector3 position = args.GetNext<Vector3>();
		transform.position = position;

		SpaceShip.SetDestination(position);
	}
}
