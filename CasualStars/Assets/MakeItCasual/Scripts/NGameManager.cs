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

    public Dictionary<uint, MovementBehavior> playerShipPair = new Dictionary<uint, MovementBehavior>();

    void Start()
    {
        if(manager != null)
        {
            networkObject.Destroy();
            return;
        }

        manager = this;

        networkObject.Networker.playerConnected += Networker_playerConnected;
        networkObject.Networker.playerDisconnected += Networker_playerDisconnected;
    }

    private void Networker_playerConnected(BeardedManStudios.Forge.Networking.NetworkingPlayer player, BeardedManStudios.Forge.Networking.NetWorker sender)
    {
        if (playerShipPair.ContainsKey(player.NetworkId)) return;

        MovementBehavior ship = spawnPlayer();
        //ship.gameObject.GetComponent<Camera>().enabled = true;

        playerShipPair.Add(player.NetworkId, ship);
    }

    private void Networker_playerDisconnected(BeardedManStudios.Forge.Networking.NetworkingPlayer player, BeardedManStudios.Forge.Networking.NetWorker sender)
    {
        if (!playerShipPair.ContainsKey(player.NetworkId)) return;

        playerShipPair.Remove(player.NetworkId);
    }

    MovementBehavior spawnPlayer()
    {
        Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), 3, Random.Range(-size.z / 2, size.z / 2));

        while (Physics.CheckSphere(pos, spawnRadius))
        {
            pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), 3, Random.Range(-size.z / 2, size.z / 2));
        }

        return NetworkManager.Instance.InstantiateMovement(0, pos, Quaternion.identity);
    }

    public MovementBehavior getShip(uint id)
    {
        MovementBehavior test;
        playerShipPair.TryGetValue(id, out test);
        return test;
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
        Movement movement = (Movement)getShip(networkObject.Networker.GetPlayerById(networkObject.MyPlayerId).NetworkId);

        Debug.Log(movement);

        movement.move(args.GetNext<Vector3>());
    }

    public override void spaceShipHyperdrive(RpcArgs args)
    {
        Movement movement = (Movement)getShip(networkObject.Networker.GetPlayerById(networkObject.MyPlayerId).NetworkId);

        Debug.Log(movement);

        movement.hyperdrive(args.GetNext<Vector3>());
    }

    public void ExecuteMove(Vector3 position)
    {
        Debug.Log(position);
        networkObject.SendRpc(RPC_SPACE_SHIP_MOVE, Receivers.Server, position);
    }

    public void ExecuteHyperDrive(Vector3 position)
    {
        Debug.Log(position);
        networkObject.SendRpc(RPC_SPACE_SHIP_HYPERDRIVE, Receivers.Server, position);
    }
}
