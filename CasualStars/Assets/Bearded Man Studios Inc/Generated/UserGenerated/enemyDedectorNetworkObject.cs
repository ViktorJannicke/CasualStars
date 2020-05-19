using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0]")]
	public partial class enemyDedectorNetworkObject : NetworkObject
	{
		public const int IDENTITY = 1;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private byte _dummyDedector;
		public event FieldEvent<byte> dummyDedectorChanged;
		public Interpolated<byte> dummyDedectorInterpolation = new Interpolated<byte>() { LerpT = 0f, Enabled = false };
		public byte dummyDedector
		{
			get { return _dummyDedector; }
			set
			{
				// Don't do anything if the value is the same
				if (_dummyDedector == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_dummyDedector = value;
				hasDirtyFields = true;
			}
		}

		public void SetdummyDedectorDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_dummyDedector(ulong timestep)
		{
			if (dummyDedectorChanged != null) dummyDedectorChanged(_dummyDedector, timestep);
			if (fieldAltered != null) fieldAltered("dummyDedector", _dummyDedector, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			dummyDedectorInterpolation.current = dummyDedectorInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _dummyDedector);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_dummyDedector = UnityObjectMapper.Instance.Map<byte>(payload);
			dummyDedectorInterpolation.current = _dummyDedector;
			dummyDedectorInterpolation.target = _dummyDedector;
			RunChange_dummyDedector(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _dummyDedector);

			// Reset all the dirty fields
			for (int i = 0; i < _dirtyFields.Length; i++)
				_dirtyFields[i] = 0;

			return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (dummyDedectorInterpolation.Enabled)
				{
					dummyDedectorInterpolation.target = UnityObjectMapper.Instance.Map<byte>(data);
					dummyDedectorInterpolation.Timestep = timestep;
				}
				else
				{
					_dummyDedector = UnityObjectMapper.Instance.Map<byte>(data);
					RunChange_dummyDedector(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (dummyDedectorInterpolation.Enabled && !dummyDedectorInterpolation.current.UnityNear(dummyDedectorInterpolation.target, 0.0015f))
			{
				_dummyDedector = (byte)dummyDedectorInterpolation.Interpolate();
				//RunChange_dummyDedector(dummyDedectorInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public enemyDedectorNetworkObject() : base() { Initialize(); }
		public enemyDedectorNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public enemyDedectorNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
