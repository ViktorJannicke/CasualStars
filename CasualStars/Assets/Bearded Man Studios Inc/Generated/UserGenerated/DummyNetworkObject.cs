using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0]")]
	public partial class DummyNetworkObject : NetworkObject
	{
		public const int IDENTITY = 5;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private byte _dummy;
		public event FieldEvent<byte> dummyChanged;
		public Interpolated<byte> dummyInterpolation = new Interpolated<byte>() { LerpT = 0f, Enabled = false };
		public byte dummy
		{
			get { return _dummy; }
			set
			{
				// Don't do anything if the value is the same
				if (_dummy == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_dummy = value;
				hasDirtyFields = true;
			}
		}

		public void SetdummyDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_dummy(ulong timestep)
		{
			if (dummyChanged != null) dummyChanged(_dummy, timestep);
			if (fieldAltered != null) fieldAltered("dummy", _dummy, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			dummyInterpolation.current = dummyInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _dummy);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_dummy = UnityObjectMapper.Instance.Map<byte>(payload);
			dummyInterpolation.current = _dummy;
			dummyInterpolation.target = _dummy;
			RunChange_dummy(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _dummy);

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
				if (dummyInterpolation.Enabled)
				{
					dummyInterpolation.target = UnityObjectMapper.Instance.Map<byte>(data);
					dummyInterpolation.Timestep = timestep;
				}
				else
				{
					_dummy = UnityObjectMapper.Instance.Map<byte>(data);
					RunChange_dummy(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (dummyInterpolation.Enabled && !dummyInterpolation.current.UnityNear(dummyInterpolation.target, 0.0015f))
			{
				_dummy = (byte)dummyInterpolation.Interpolate();
				//RunChange_dummy(dummyInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public DummyNetworkObject() : base() { Initialize(); }
		public DummyNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public DummyNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
