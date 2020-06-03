using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0.15,0.15,0,0]")]
	public partial class MovementNetworkObject : NetworkObject
	{
		public const int IDENTITY = 6;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private Vector3 _position;
		public event FieldEvent<Vector3> positionChanged;
		public InterpolateVector3 positionInterpolation = new InterpolateVector3() { LerpT = 0.15f, Enabled = true };
		public Vector3 position
		{
			get { return _position; }
			set
			{
				// Don't do anything if the value is the same
				if (_position == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_position = value;
				hasDirtyFields = true;
			}
		}

		public void SetpositionDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_position(ulong timestep)
		{
			if (positionChanged != null) positionChanged(_position, timestep);
			if (fieldAltered != null) fieldAltered("position", _position, timestep);
		}
		[ForgeGeneratedField]
		private Quaternion _rotation;
		public event FieldEvent<Quaternion> rotationChanged;
		public InterpolateQuaternion rotationInterpolation = new InterpolateQuaternion() { LerpT = 0.15f, Enabled = true };
		public Quaternion rotation
		{
			get { return _rotation; }
			set
			{
				// Don't do anything if the value is the same
				if (_rotation == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_rotation = value;
				hasDirtyFields = true;
			}
		}

		public void SetrotationDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_rotation(ulong timestep)
		{
			if (rotationChanged != null) rotationChanged(_rotation, timestep);
			if (fieldAltered != null) fieldAltered("rotation", _rotation, timestep);
		}
		[ForgeGeneratedField]
		private uint _myPlayerID;
		public event FieldEvent<uint> myPlayerIDChanged;
		public Interpolated<uint> myPlayerIDInterpolation = new Interpolated<uint>() { LerpT = 0f, Enabled = false };
		public uint myPlayerID
		{
			get { return _myPlayerID; }
			set
			{
				// Don't do anything if the value is the same
				if (_myPlayerID == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x4;
				_myPlayerID = value;
				hasDirtyFields = true;
			}
		}

		public void SetmyPlayerIDDirty()
		{
			_dirtyFields[0] |= 0x4;
			hasDirtyFields = true;
		}

		private void RunChange_myPlayerID(ulong timestep)
		{
			if (myPlayerIDChanged != null) myPlayerIDChanged(_myPlayerID, timestep);
			if (fieldAltered != null) fieldAltered("myPlayerID", _myPlayerID, timestep);
		}
		[ForgeGeneratedField]
		private int _myPlayerDataID;
		public event FieldEvent<int> myPlayerDataIDChanged;
		public Interpolated<int> myPlayerDataIDInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int myPlayerDataID
		{
			get { return _myPlayerDataID; }
			set
			{
				// Don't do anything if the value is the same
				if (_myPlayerDataID == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x8;
				_myPlayerDataID = value;
				hasDirtyFields = true;
			}
		}

		public void SetmyPlayerDataIDDirty()
		{
			_dirtyFields[0] |= 0x8;
			hasDirtyFields = true;
		}

		private void RunChange_myPlayerDataID(ulong timestep)
		{
			if (myPlayerDataIDChanged != null) myPlayerDataIDChanged(_myPlayerDataID, timestep);
			if (fieldAltered != null) fieldAltered("myPlayerDataID", _myPlayerDataID, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			positionInterpolation.current = positionInterpolation.target;
			rotationInterpolation.current = rotationInterpolation.target;
			myPlayerIDInterpolation.current = myPlayerIDInterpolation.target;
			myPlayerDataIDInterpolation.current = myPlayerDataIDInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _position);
			UnityObjectMapper.Instance.MapBytes(data, _rotation);
			UnityObjectMapper.Instance.MapBytes(data, _myPlayerID);
			UnityObjectMapper.Instance.MapBytes(data, _myPlayerDataID);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_position = UnityObjectMapper.Instance.Map<Vector3>(payload);
			positionInterpolation.current = _position;
			positionInterpolation.target = _position;
			RunChange_position(timestep);
			_rotation = UnityObjectMapper.Instance.Map<Quaternion>(payload);
			rotationInterpolation.current = _rotation;
			rotationInterpolation.target = _rotation;
			RunChange_rotation(timestep);
			_myPlayerID = UnityObjectMapper.Instance.Map<uint>(payload);
			myPlayerIDInterpolation.current = _myPlayerID;
			myPlayerIDInterpolation.target = _myPlayerID;
			RunChange_myPlayerID(timestep);
			_myPlayerDataID = UnityObjectMapper.Instance.Map<int>(payload);
			myPlayerDataIDInterpolation.current = _myPlayerDataID;
			myPlayerDataIDInterpolation.target = _myPlayerDataID;
			RunChange_myPlayerDataID(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _position);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _rotation);
			if ((0x4 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _myPlayerID);
			if ((0x8 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _myPlayerDataID);

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
				if (positionInterpolation.Enabled)
				{
					positionInterpolation.target = UnityObjectMapper.Instance.Map<Vector3>(data);
					positionInterpolation.Timestep = timestep;
				}
				else
				{
					_position = UnityObjectMapper.Instance.Map<Vector3>(data);
					RunChange_position(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (rotationInterpolation.Enabled)
				{
					rotationInterpolation.target = UnityObjectMapper.Instance.Map<Quaternion>(data);
					rotationInterpolation.Timestep = timestep;
				}
				else
				{
					_rotation = UnityObjectMapper.Instance.Map<Quaternion>(data);
					RunChange_rotation(timestep);
				}
			}
			if ((0x4 & readDirtyFlags[0]) != 0)
			{
				if (myPlayerIDInterpolation.Enabled)
				{
					myPlayerIDInterpolation.target = UnityObjectMapper.Instance.Map<uint>(data);
					myPlayerIDInterpolation.Timestep = timestep;
				}
				else
				{
					_myPlayerID = UnityObjectMapper.Instance.Map<uint>(data);
					RunChange_myPlayerID(timestep);
				}
			}
			if ((0x8 & readDirtyFlags[0]) != 0)
			{
				if (myPlayerDataIDInterpolation.Enabled)
				{
					myPlayerDataIDInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					myPlayerDataIDInterpolation.Timestep = timestep;
				}
				else
				{
					_myPlayerDataID = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_myPlayerDataID(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (positionInterpolation.Enabled && !positionInterpolation.current.UnityNear(positionInterpolation.target, 0.0015f))
			{
				_position = (Vector3)positionInterpolation.Interpolate();
				//RunChange_position(positionInterpolation.Timestep);
			}
			if (rotationInterpolation.Enabled && !rotationInterpolation.current.UnityNear(rotationInterpolation.target, 0.0015f))
			{
				_rotation = (Quaternion)rotationInterpolation.Interpolate();
				//RunChange_rotation(rotationInterpolation.Timestep);
			}
			if (myPlayerIDInterpolation.Enabled && !myPlayerIDInterpolation.current.UnityNear(myPlayerIDInterpolation.target, 0.0015f))
			{
				_myPlayerID = (uint)myPlayerIDInterpolation.Interpolate();
				//RunChange_myPlayerID(myPlayerIDInterpolation.Timestep);
			}
			if (myPlayerDataIDInterpolation.Enabled && !myPlayerDataIDInterpolation.current.UnityNear(myPlayerDataIDInterpolation.target, 0.0015f))
			{
				_myPlayerDataID = (int)myPlayerDataIDInterpolation.Interpolate();
				//RunChange_myPlayerDataID(myPlayerDataIDInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public MovementNetworkObject() : base() { Initialize(); }
		public MovementNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public MovementNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
