using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NGameManager : NetworkedGameManagerBehavior
{
    public static NGameManager manager;

    public Dictionary<uint, MovementBehavior> playerShipPair = new Dictionary<uint, MovementBehavior>();

    // Start is called before the first frame update
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

        playerShipPair.Add(player.NetworkId, ship);
    }

    private void Networker_playerDisconnected(BeardedManStudios.Forge.Networking.NetworkingPlayer player, BeardedManStudios.Forge.Networking.NetWorker sender)
    {
        if (!playerShipPair.ContainsKey(player.NetworkId)) return;

        playerShipPair.Remove(player.NetworkId);
    }

    MovementBehavior spawnPlayer()
    {
        return NetworkManager.Instance.InstantiateMovement(0, transform.position, transform.rotation);
    }

    public MovementBehavior getShip(uint id)
    {
        MovementBehavior test;
        playerShipPair.TryGetValue(id, out test);
        return test;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
