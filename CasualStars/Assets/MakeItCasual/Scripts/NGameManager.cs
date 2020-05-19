using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NGameManager : NetworkedGameManagerBehavior
{
    public Dictionary<uint, MovementBehavior> playerShipPair = new Dictionary<uint, MovementBehavior>();

    // Start is called before the first frame update
    void Start()
    {
        networkObject.Networker.playerConnected += Networker_playerConnected;
        networkObject.Networker.playerDisconnected += Networker_playerDisconnected;
    }

    private void Networker_playerConnected(BeardedManStudios.Forge.Networking.NetworkingPlayer player, BeardedManStudios.Forge.Networking.NetWorker sender)
    {
        if (playerShipPair.ContainsKey(player.NetworkId)) return;

        MovementBehavior ship = spawnPlayer();

        playerShipPair.Add(player.NetworkId, ship);
        Debug.Log(player.NetworkId);
    }

    private void Networker_playerDisconnected(BeardedManStudios.Forge.Networking.NetworkingPlayer player, BeardedManStudios.Forge.Networking.NetWorker sender)
    {
        if (!playerShipPair.ContainsKey(player.NetworkId)) return;

        playerShipPair.Remove(player.NetworkId);
    }

    MovementBehavior spawnPlayer ()
    {
        return NetworkManager.Instance.InstantiateMovement(0, transform.position, transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
