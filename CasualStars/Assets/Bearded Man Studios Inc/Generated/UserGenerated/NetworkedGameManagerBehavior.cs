using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedRPC("{\"types\":[[\"uint\", \"Vector3\"][\"uint\", \"Vector3\"][\"uint\"][\"uint\"][\"uint\", \"string\"]]")]
	[GeneratedRPCVariableNames("{\"types\":[[\"PlayerID\", \"position\"][\"PlayerID\", \"position\"][\"PlayerID\"][\"networkID\"][\"networkID\", \"DataID\"]]")]
	public abstract partial class NetworkedGameManagerBehavior : NetworkBehavior
	{
		public const byte RPC_SPACE_SHIP_MOVE = 0 + 5;
		public const byte RPC_SPACE_SHIP_HYPERDRIVE = 1 + 5;
		public const byte RPC_SPAWN_MY_SHIP = 2 + 5;
		public const byte RPC_GET_PLAYER_DATA_I_D = 3 + 5;
		public const byte RPC_SEND_PLAYERS_DATA_I_D = 4 + 5;
		
		public NetworkedGameManagerNetworkObject networkObject = null;

		public override void Initialize(NetworkObject obj)
		{
			// We have already initialized this object
			if (networkObject != null && networkObject.AttachedBehavior != null)
				return;
			
			networkObject = (NetworkedGameManagerNetworkObject)obj;
			networkObject.AttachedBehavior = this;

			base.SetupHelperRpcs(networkObject);
			networkObject.RegisterRpc("spaceShipMove", spaceShipMove, typeof(uint), typeof(Vector3));
			networkObject.RegisterRpc("spaceShipHyperdrive", spaceShipHyperdrive, typeof(uint), typeof(Vector3));
			networkObject.RegisterRpc("spawnMyShip", spawnMyShip, typeof(uint));
			networkObject.RegisterRpc("GetPlayerDataID", GetPlayerDataID, typeof(uint));
			networkObject.RegisterRpc("SendPlayersDataID", SendPlayersDataID, typeof(uint), typeof(string));

			networkObject.onDestroy += DestroyGameObject;

			if (!obj.IsOwner)
			{
				if (!skipAttachIds.ContainsKey(obj.NetworkId)){
					uint newId = obj.NetworkId + 1;
					ProcessOthers(gameObject.transform, ref newId);
				}
				else
					skipAttachIds.Remove(obj.NetworkId);
			}

			if (obj.Metadata != null)
			{
				byte transformFlags = obj.Metadata[0];

				if (transformFlags != 0)
				{
					BMSByte metadataTransform = new BMSByte();
					metadataTransform.Clone(obj.Metadata);
					metadataTransform.MoveStartIndex(1);

					if ((transformFlags & 0x01) != 0 && (transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() =>
						{
							transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform);
							transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform);
						});
					}
					else if ((transformFlags & 0x01) != 0)
					{
						MainThreadManager.Run(() => { transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform); });
					}
					else if ((transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() => { transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform); });
					}
				}
			}

			MainThreadManager.Run(() =>
			{
				NetworkStart();
				networkObject.Networker.FlushCreateActions(networkObject);
			});
		}

		protected override void CompleteRegistration()
		{
			base.CompleteRegistration();
			networkObject.ReleaseCreateBuffer();
		}

		public override void Initialize(NetWorker networker, byte[] metadata = null)
		{
			Initialize(new NetworkedGameManagerNetworkObject(networker, createCode: TempAttachCode, metadata: metadata));
		}

		private void DestroyGameObject(NetWorker sender)
		{
			MainThreadManager.Run(() => { try { Destroy(gameObject); } catch { } });
			networkObject.onDestroy -= DestroyGameObject;
		}

		public override NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null)
		{
			return new NetworkedGameManagerNetworkObject(networker, this, createCode, metadata);
		}

		protected override void InitializedTransform()
		{
			networkObject.SnapInterpolations();
		}

		/// <summary>
		/// Arguments:
		/// uint PlayerID
		/// Vector3 position
		/// </summary>
		public abstract void spaceShipMove(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// uint PlayerID
		/// Vector3 position
		/// </summary>
		public abstract void spaceShipHyperdrive(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// uint PlayerID
		/// </summary>
		public abstract void spawnMyShip(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// uint networkID
		/// </summary>
		public abstract void GetPlayerDataID(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// uint networkID
		/// string DataID
		/// </summary>
		public abstract void SendPlayersDataID(RpcArgs args);

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}