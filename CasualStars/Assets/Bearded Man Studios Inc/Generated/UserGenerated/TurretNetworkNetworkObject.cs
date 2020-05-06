using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0.15,0.15,0.15,0.15]")]
	public partial class TurretNetworkNetworkObject : NetworkObject
	{
		public const int IDENTITY = 10;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private Quaternion _turretRotation;
		public event FieldEvent<Quaternion> turretRotationChanged;
		public InterpolateQuaternion turretRotationInterpolation = new InterpolateQuaternion() { LerpT = 0.15f, Enabled = true };
		public Quaternion turretRotation
		{
			get { return _turretRotation; }
			set
			{
				// Don't do anything if the value is the same
				if (_turretRotation == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_turretRotation = value;
				hasDirtyFields = true;
			}
		}

		public void SetturretRotationDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_turretRotation(ulong timestep)
		{
			if (turretRotationChanged != null) turretRotationChanged(_turretRotation, timestep);
			if (fieldAltered != null) fieldAltered("turretRotation", _turretRotation, timestep);
		}
		[ForgeGeneratedField]
		private Vector3 _turretPosition;
		public event FieldEvent<Vector3> turretPositionChanged;
		public InterpolateVector3 turretPositionInterpolation = new InterpolateVector3() { LerpT = 0.15f, Enabled = true };
		public Vector3 turretPosition
		{
			get { return _turretPosition; }
			set
			{
				// Don't do anything if the value is the same
				if (_turretPosition == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_turretPosition = value;
				hasDirtyFields = true;
			}
		}

		public void SetturretPositionDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_turretPosition(ulong timestep)
		{
			if (turretPositionChanged != null) turretPositionChanged(_turretPosition, timestep);
			if (fieldAltered != null) fieldAltered("turretPosition", _turretPosition, timestep);
		}
		[ForgeGeneratedField]
		private Quaternion _handleRotation;
		public event FieldEvent<Quaternion> handleRotationChanged;
		public InterpolateQuaternion handleRotationInterpolation = new InterpolateQuaternion() { LerpT = 0.15f, Enabled = true };
		public Quaternion handleRotation
		{
			get { return _handleRotation; }
			set
			{
				// Don't do anything if the value is the same
				if (_handleRotation == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x4;
				_handleRotation = value;
				hasDirtyFields = true;
			}
		}

		public void SethandleRotationDirty()
		{
			_dirtyFields[0] |= 0x4;
			hasDirtyFields = true;
		}

		private void RunChange_handleRotation(ulong timestep)
		{
			if (handleRotationChanged != null) handleRotationChanged(_handleRotation, timestep);
			if (fieldAltered != null) fieldAltered("handleRotation", _handleRotation, timestep);
		}
		[ForgeGeneratedField]
		private Vector3 _handlePosition;
		public event FieldEvent<Vector3> handlePositionChanged;
		public InterpolateVector3 handlePositionInterpolation = new InterpolateVector3() { LerpT = 0.15f, Enabled = true };
		public Vector3 handlePosition
		{
			get { return _handlePosition; }
			set
			{
				// Don't do anything if the value is the same
				if (_handlePosition == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x8;
				_handlePosition = value;
				hasDirtyFields = true;
			}
		}

		public void SethandlePositionDirty()
		{
			_dirtyFields[0] |= 0x8;
			hasDirtyFields = true;
		}

		private void RunChange_handlePosition(ulong timestep)
		{
			if (handlePositionChanged != null) handlePositionChanged(_handlePosition, timestep);
			if (fieldAltered != null) fieldAltered("handlePosition", _handlePosition, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			turretRotationInterpolation.current = turretRotationInterpolation.target;
			turretPositionInterpolation.current = turretPositionInterpolation.target;
			handleRotationInterpolation.current = handleRotationInterpolation.target;
			handlePositionInterpolation.current = handlePositionInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _turretRotation);
			UnityObjectMapper.Instance.MapBytes(data, _turretPosition);
			UnityObjectMapper.Instance.MapBytes(data, _handleRotation);
			UnityObjectMapper.Instance.MapBytes(data, _handlePosition);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_turretRotation = UnityObjectMapper.Instance.Map<Quaternion>(payload);
			turretRotationInterpolation.current = _turretRotation;
			turretRotationInterpolation.target = _turretRotation;
			RunChange_turretRotation(timestep);
			_turretPosition = UnityObjectMapper.Instance.Map<Vector3>(payload);
			turretPositionInterpolation.current = _turretPosition;
			turretPositionInterpolation.target = _turretPosition;
			RunChange_turretPosition(timestep);
			_handleRotation = UnityObjectMapper.Instance.Map<Quaternion>(payload);
			handleRotationInterpolation.current = _handleRotation;
			handleRotationInterpolation.target = _handleRotation;
			RunChange_handleRotation(timestep);
			_handlePosition = UnityObjectMapper.Instance.Map<Vector3>(payload);
			handlePositionInterpolation.current = _handlePosition;
			handlePositionInterpolation.target = _handlePosition;
			RunChange_handlePosition(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _turretRotation);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _turretPosition);
			if ((0x4 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _handleRotation);
			if ((0x8 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _handlePosition);

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
				if (turretRotationInterpolation.Enabled)
				{
					turretRotationInterpolation.target = UnityObjectMapper.Instance.Map<Quaternion>(data);
					turretRotationInterpolation.Timestep = timestep;
				}
				else
				{
					_turretRotation = UnityObjectMapper.Instance.Map<Quaternion>(data);
					RunChange_turretRotation(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (turretPositionInterpolation.Enabled)
				{
					turretPositionInterpolation.target = UnityObjectMapper.Instance.Map<Vector3>(data);
					turretPositionInterpolation.Timestep = timestep;
				}
				else
				{
					_turretPosition = UnityObjectMapper.Instance.Map<Vector3>(data);
					RunChange_turretPosition(timestep);
				}
			}
			if ((0x4 & readDirtyFlags[0]) != 0)
			{
				if (handleRotationInterpolation.Enabled)
				{
					handleRotationInterpolation.target = UnityObjectMapper.Instance.Map<Quaternion>(data);
					handleRotationInterpolation.Timestep = timestep;
				}
				else
				{
					_handleRotation = UnityObjectMapper.Instance.Map<Quaternion>(data);
					RunChange_handleRotation(timestep);
				}
			}
			if ((0x8 & readDirtyFlags[0]) != 0)
			{
				if (handlePositionInterpolation.Enabled)
				{
					handlePositionInterpolation.target = UnityObjectMapper.Instance.Map<Vector3>(data);
					handlePositionInterpolation.Timestep = timestep;
				}
				else
				{
					_handlePosition = UnityObjectMapper.Instance.Map<Vector3>(data);
					RunChange_handlePosition(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (turretRotationInterpolation.Enabled && !turretRotationInterpolation.current.UnityNear(turretRotationInterpolation.target, 0.0015f))
			{
				_turretRotation = (Quaternion)turretRotationInterpolation.Interpolate();
				//RunChange_turretRotation(turretRotationInterpolation.Timestep);
			}
			if (turretPositionInterpolation.Enabled && !turretPositionInterpolation.current.UnityNear(turretPositionInterpolation.target, 0.0015f))
			{
				_turretPosition = (Vector3)turretPositionInterpolation.Interpolate();
				//RunChange_turretPosition(turretPositionInterpolation.Timestep);
			}
			if (handleRotationInterpolation.Enabled && !handleRotationInterpolation.current.UnityNear(handleRotationInterpolation.target, 0.0015f))
			{
				_handleRotation = (Quaternion)handleRotationInterpolation.Interpolate();
				//RunChange_handleRotation(handleRotationInterpolation.Timestep);
			}
			if (handlePositionInterpolation.Enabled && !handlePositionInterpolation.current.UnityNear(handlePositionInterpolation.target, 0.0015f))
			{
				_handlePosition = (Vector3)handlePositionInterpolation.Interpolate();
				//RunChange_handlePosition(handlePositionInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public TurretNetworkNetworkObject() : base() { Initialize(); }
		public TurretNetworkNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public TurretNetworkNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
