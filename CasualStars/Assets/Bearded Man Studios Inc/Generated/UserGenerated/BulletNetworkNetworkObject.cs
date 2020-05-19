using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0.15,0.15]")]
	public partial class BulletNetworkNetworkObject : NetworkObject
	{
		public const int IDENTITY = 1;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private Quaternion _bulletRotation;
		public event FieldEvent<Quaternion> bulletRotationChanged;
		public InterpolateQuaternion bulletRotationInterpolation = new InterpolateQuaternion() { LerpT = 0.15f, Enabled = true };
		public Quaternion bulletRotation
		{
			get { return _bulletRotation; }
			set
			{
				// Don't do anything if the value is the same
				if (_bulletRotation == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_bulletRotation = value;
				hasDirtyFields = true;
			}
		}

		public void SetbulletRotationDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_bulletRotation(ulong timestep)
		{
			if (bulletRotationChanged != null) bulletRotationChanged(_bulletRotation, timestep);
			if (fieldAltered != null) fieldAltered("bulletRotation", _bulletRotation, timestep);
		}
		[ForgeGeneratedField]
		private Vector3 _bulletPosition;
		public event FieldEvent<Vector3> bulletPositionChanged;
		public InterpolateVector3 bulletPositionInterpolation = new InterpolateVector3() { LerpT = 0.15f, Enabled = true };
		public Vector3 bulletPosition
		{
			get { return _bulletPosition; }
			set
			{
				// Don't do anything if the value is the same
				if (_bulletPosition == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_bulletPosition = value;
				hasDirtyFields = true;
			}
		}

		public void SetbulletPositionDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_bulletPosition(ulong timestep)
		{
			if (bulletPositionChanged != null) bulletPositionChanged(_bulletPosition, timestep);
			if (fieldAltered != null) fieldAltered("bulletPosition", _bulletPosition, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			bulletRotationInterpolation.current = bulletRotationInterpolation.target;
			bulletPositionInterpolation.current = bulletPositionInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _bulletRotation);
			UnityObjectMapper.Instance.MapBytes(data, _bulletPosition);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_bulletRotation = UnityObjectMapper.Instance.Map<Quaternion>(payload);
			bulletRotationInterpolation.current = _bulletRotation;
			bulletRotationInterpolation.target = _bulletRotation;
			RunChange_bulletRotation(timestep);
			_bulletPosition = UnityObjectMapper.Instance.Map<Vector3>(payload);
			bulletPositionInterpolation.current = _bulletPosition;
			bulletPositionInterpolation.target = _bulletPosition;
			RunChange_bulletPosition(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _bulletRotation);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _bulletPosition);

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
				if (bulletRotationInterpolation.Enabled)
				{
					bulletRotationInterpolation.target = UnityObjectMapper.Instance.Map<Quaternion>(data);
					bulletRotationInterpolation.Timestep = timestep;
				}
				else
				{
					_bulletRotation = UnityObjectMapper.Instance.Map<Quaternion>(data);
					RunChange_bulletRotation(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (bulletPositionInterpolation.Enabled)
				{
					bulletPositionInterpolation.target = UnityObjectMapper.Instance.Map<Vector3>(data);
					bulletPositionInterpolation.Timestep = timestep;
				}
				else
				{
					_bulletPosition = UnityObjectMapper.Instance.Map<Vector3>(data);
					RunChange_bulletPosition(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (bulletRotationInterpolation.Enabled && !bulletRotationInterpolation.current.UnityNear(bulletRotationInterpolation.target, 0.0015f))
			{
				_bulletRotation = (Quaternion)bulletRotationInterpolation.Interpolate();
				//RunChange_bulletRotation(bulletRotationInterpolation.Timestep);
			}
			if (bulletPositionInterpolation.Enabled && !bulletPositionInterpolation.current.UnityNear(bulletPositionInterpolation.target, 0.0015f))
			{
				_bulletPosition = (Vector3)bulletPositionInterpolation.Interpolate();
				//RunChange_bulletPosition(bulletPositionInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public BulletNetworkNetworkObject() : base() { Initialize(); }
		public BulletNetworkNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public BulletNetworkNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
