using BeardedManStudios.Forge.Networking.Generated;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Unity
{
	public partial class NetworkManager : MonoBehaviour
	{
		public delegate void InstantiateEvent(INetworkBehavior unityGameObject, NetworkObject obj);
		public event InstantiateEvent objectInitialized;
		protected BMSByte metadata = new BMSByte();

		public GameObject[] enemyDedectorNetworkObject = null;

		protected virtual void SetupObjectCreatedEvent()
		{
			Networker.objectCreated += CaptureObjects;
		}

		protected virtual void OnDestroy()
		{
		    if (Networker != null)
				Networker.objectCreated -= CaptureObjects;
		}
		
		private void CaptureObjects(NetworkObject obj)
		{
			if (obj.CreateCode < 0)
				return;
				
			if (obj is enemyDedectorNetworkObject)
			{
				MainThreadManager.Run(() =>
				{
					NetworkBehavior newObj = null;
					if (!NetworkBehavior.skipAttachIds.TryGetValue(obj.NetworkId, out newObj))
					{
						if (enemyDedectorNetworkObject.Length > 0 && enemyDedectorNetworkObject[obj.CreateCode] != null)
						{
							var go = Instantiate(enemyDedectorNetworkObject[obj.CreateCode]);
							newObj = go.GetComponent<enemyDedectorBehavior>();
						}
					}

					if (newObj == null)
						return;
						
					newObj.Initialize(obj);

					if (objectInitialized != null)
						objectInitialized(newObj, obj);
				});
			}
		}

		protected virtual void InitializedObject(INetworkBehavior behavior, NetworkObject obj)
		{
			if (objectInitialized != null)
				objectInitialized(behavior, obj);

			obj.pendingInitialized -= InitializedObject;
		}

		[Obsolete("Use InstantiateenemyDedector instead, its shorter and easier to type out ;)")]
		public enemyDedectorBehavior InstantiateenemyDedectorNetworkObject(int index = 0, Vector3? position = null, Quaternion? rotation = null, bool sendTransform = true)
		{
			var go = Instantiate(enemyDedectorNetworkObject[index]);
			var netBehavior = go.GetComponent<enemyDedectorBehavior>();
			var obj = netBehavior.CreateNetworkObject(Networker, index);
			go.GetComponent<enemyDedectorBehavior>().networkObject = (enemyDedectorNetworkObject)obj;

			FinalizeInitialization(go, netBehavior, obj, position, rotation, sendTransform);
			
			return netBehavior;
		}

		/// <summary>
		/// Instantiate an instance of enemyDedector
		/// </summary>
		/// <returns>
		/// A local instance of enemyDedectorBehavior
		/// </returns>
		/// <param name="index">The index of the enemyDedector prefab in the NetworkManager to Instantiate</param>
		/// <param name="position">Optional parameter which defines the position of the created GameObject</param>
		/// <param name="rotation">Optional parameter which defines the rotation of the created GameObject</param>
		/// <param name="sendTransform">Optional Parameter to send transform data to other connected clients on Instantiation</param>
		public enemyDedectorBehavior InstantiateenemyDedector(int index = 0, Vector3? position = null, Quaternion? rotation = null, bool sendTransform = true)
		{
			var go = Instantiate(enemyDedectorNetworkObject[index]);
			var netBehavior = go.GetComponent<enemyDedectorBehavior>();

			NetworkObject obj = null;
			if (!sendTransform && position == null && rotation == null)
				obj = netBehavior.CreateNetworkObject(Networker, index);
			else
			{
				metadata.Clear();

				if (position == null && rotation == null)
				{
					byte transformFlags = 0x1 | 0x2;
					ObjectMapper.Instance.MapBytes(metadata, transformFlags);
					ObjectMapper.Instance.MapBytes(metadata, go.transform.position, go.transform.rotation);
				}
				else
				{
					byte transformFlags = 0x0;
					transformFlags |= (byte)(position != null ? 0x1 : 0x0);
					transformFlags |= (byte)(rotation != null ? 0x2 : 0x0);
					ObjectMapper.Instance.MapBytes(metadata, transformFlags);

					if (position != null)
						ObjectMapper.Instance.MapBytes(metadata, position.Value);

					if (rotation != null)
						ObjectMapper.Instance.MapBytes(metadata, rotation.Value);
				}

				obj = netBehavior.CreateNetworkObject(Networker, index, metadata.CompressBytes());
			}

			go.GetComponent<enemyDedectorBehavior>().networkObject = (enemyDedectorNetworkObject)obj;

			FinalizeInitialization(go, netBehavior, obj, position, rotation, sendTransform);
			
			return netBehavior;
		}
	}
}
