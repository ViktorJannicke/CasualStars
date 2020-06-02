using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NGameManager : NetworkedGameManagerBehavior
{
    public static NGameManager manager;

    public GameObject Asteroidprefab;
    public Vector3 center;
    public Vector3 size;
    public bool stopSpawning = false;
    public int spawnTime;
    public int spawnDelay;
    public int spawnRadius;

    public GameObject myCamera;

    public Dictionary<uint, Movement> playerShipPair = new Dictionary<uint, Movement>();

    void Start()
    {
        if(manager != null)
        {
            networkObject.Destroy();
            return;
        }

        manager = this;

        if (networkObject.IsServer)
        {
            networkObject.Networker.playerDisconnected += Networker_playerDisconnected;
            Instantiate(myCamera);
        }

        if(!networkObject.IsServer)
        {
            networkObject.SendRpc(RPC_SPAWN_MY_SHIP, Receivers.Server, networkObject.Networker.Me.NetworkId);
        }
    }

    private void Networker_playerDisconnected(BeardedManStudios.Forge.Networking.NetworkingPlayer player, BeardedManStudios.Forge.Networking.NetWorker sender)
    {
        if (!networkObject.IsServer) return;

        if (!playerShipPair.ContainsKey(player.NetworkId)) return;

        Movement movement;
        playerShipPair.TryGetValue(player.NetworkId, out movement);
        movement.networkObject.Destroy();

        playerShipPair.Remove(player.NetworkId);
    }

    Movement spawnPlayer()
    {
        Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), 3, Random.Range(-size.z / 2, size.z / 2));

        while (Physics.CheckSphere(pos, spawnRadius))
        {
            pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), 3, Random.Range(-size.z / 2, size.z / 2));
        }

        MovementBehavior bh = NetworkManager.Instance.InstantiateMovement(0, pos, Quaternion.identity);

        return (Movement)bh;
    }

    public void Asteroidsspawn()
    {
        Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));

        while (Physics.CheckSphere(pos, spawnRadius))
        {
            pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
        }
        Instantiate(Asteroidprefab, pos, Quaternion.identity);
    }

    public Vector3 getNextPosition()
    {
        Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));

        while (Physics.CheckSphere(pos, spawnRadius))
        {
            pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
        }

        return pos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        Gizmos.DrawCube(transform.localPosition + center, size);
    }

    public override void spaceShipMove(RpcArgs args)
    {
        Movement movement;
        playerShipPair.TryGetValue(args.GetNext<uint>(), out movement);

        movement.move(args.GetNext<Vector3>());
    }

    public override void spaceShipHyperdrive(RpcArgs args)
    {
        Movement movement;
        playerShipPair.TryGetValue(args.GetNext<uint>(), out movement);

        movement.hyperdrive(args.GetNext<Vector3>());
    }

    public void ExecuteMove(Vector3 position)
    {
        Debug.Log(position);

        networkObject.SendRpc(RPC_SPACE_SHIP_MOVE, Receivers.Server, networkObject.Networker.Me.NetworkId, position);
    }

    public void ExecuteHyperDrive(Vector3 position)
    {
        Debug.Log(position);
        networkObject.SendRpc(RPC_SPACE_SHIP_HYPERDRIVE, Receivers.Server, networkObject.Networker.Me.NetworkId, position);
    }

    public override void spawnMyShip(RpcArgs args)
    {
        uint pid = args.GetNext<uint>();

        if (playerShipPair.ContainsKey(pid)) return;

        Movement ship = spawnPlayer();
        //ship.gameObject.GetComponent<Camera>().enabled = true;
        ship.networkObject.myPlayerID = pid;

        playerShipPair.Add(pid, ship);
    }
}
