using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Policy;

public class NGameManager : NetworkedGameManagerBehavior
{
    public static NGameManager manager;

    [Header("Player Base Values")]
    public int bHealth;
    public int bShield;
    public int bScore;
    public int bCredits;

    [Header("Player Indentifier Values")]
    public string PlayerDataID;

    [Header("Other Values")]
    public GameObject Asteroidprefab;
    public Vector3 center;
    public Vector3 size;
    public bool stopSpawning = false;
    public int spawnTime;
    public int spawnDelay;
    public int spawnRadius;

    public GameObject myCamera;

    public Dictionary<uint, Movement> playerShipPair = new Dictionary<uint, Movement>();
    public Dictionary<string, PlayerData> playerDataPair;

    public float timer = 0;
    public float maxTime = 300;

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

            if(SaveSystem.PlayerDataExists())
            {
                playerDataPair = SaveSystem.LoadPlayer();
            }
            else
            {
                playerDataPair = new Dictionary<string, PlayerData>();
                playerDataPair.Add("0", new PlayerData("0",0,0,0,0));

                SaveSystem.SavePlayer(playerDataPair);
            }
        }

        if(!networkObject.IsServer)
        {
            Instantiate(myCamera);
            //

            if (SaveSystem.PlayerDataIDExists())
            {
                PlayerDataID = SaveSystem.LoadPlayerDataID();
            }
            else
            {
                networkObject.SendRpc(RPC_GET_PLAYER_DATA_I_D, Receivers.Server, networkObject.Networker.Me.NetworkId);
            }
        }
    }

    private void Update()
    {
        if(timer >= maxTime)
        {
            timer = 0;
            SaveSystem.SavePlayer(playerDataPair);
        } else
        {
            timer += Time.deltaTime;
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

    public void startGame()
    {
        networkObject.SendRpc(RPC_SPAWN_MY_SHIP, Receivers.Server, networkObject.Networker.Me.NetworkId);
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

    public override void GetPlayerDataID(RpcArgs args)
    {
        uint networkid = args.GetNext<uint>();

        string id;

        do
        {
            id = " ";

            for (int i = 0; i < 50; i++)
            {
                id += (int)Random.Range(1, 9);
            }
        }
        while (playerDataPair.ContainsKey(id));
        playerDataPair.Add(id, new PlayerData("0", bHealth, bShield, bScore, bCredits));

        networkObject.SendRpc(RPC_SEND_PLAYERS_DATA_I_D, Receivers.Others, networkid, id);
    }

    public override void SendPlayersDataID(RpcArgs args)
    {
        uint networkid = args.GetNext<uint>();
        
        if (networkObject.Networker.Me.NetworkId == networkid)
        {
            string id = args.GetNext<string>();
            PlayerDataID = id;
            SaveSystem.SavePlayerDataID(id);
        }
    }
}