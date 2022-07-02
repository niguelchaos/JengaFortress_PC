using Unity;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetworkTransform : NetworkBehaviour
{
    // private readonly NetworkVariable<PlayerNetworkState> _playerState = new(writePerm: NetworkVariableWritePermission.Owner);

    [SerializeField] private float _cheapInterpolationTime = 0.1f;

    // toggle for client/server authority
    [SerializeField] private bool usingServerAuth;
    // network variables only changeable by server
    private NetworkVariable<PlayerNetworkState> _playerState;

    private void Awake()
    {
        var permission = usingServerAuth ? NetworkVariableWritePermission.Server : NetworkVariableWritePermission.Owner;
        _playerState = new NetworkVariable<PlayerNetworkState>(writePerm: permission);

    }

    void Update()
    {
        // either writing or reading from network
        if (IsOwner)
        {
            TransmitState();
        }
        else { 
            ConsumeState(); 
        }
    }

    #region Transmit State

    private void TransmitState() {
        var state = new PlayerNetworkState {
            Position = transform.position,
            Rotation = transform.rotation.eulerAngles
        };

        // if script not set to using server as authority
        // then just write our value
        // if the player is the server itself, then also can execute immediately
        if (IsServer || !usingServerAuth) {
            _playerState.Value = state;
        }
        else {
            // but if server authority is on, client cannot write to variable
            // need to tell server "this is my state, tell this to rest of the clients"
            // RPC = remote procedure call = code that can be run on another system
            TransmitStateServerRpc(state);
        }
    }

    [ServerRpc]
    // when this runs on server, update player state
    private void TransmitStateServerRpc(PlayerNetworkState state) {
        _playerState.Value = state;
    }

    #endregion



    struct PlayerNetworkState : INetworkSerializable
    {
        private float _xPos, _yPos, _zPos;
        private short _xRot, _yRot, _zRot;

        internal Vector3 Position
        {
            get => new Vector3(_xPos, _yPos, _zPos);
            set {
                _xPos = value.x;
                _yPos = value.y;
                _zPos = value.z;
            }
        }

        internal Vector3 Rotation 
        {
            get => new Vector3(_xRot, _yRot, _zRot);
            set {
                _xRot = (short)value.x;
                _yRot = (short)value.y;
                _zRot = (short)value.z;
            }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T: IReaderWriter
        {
            serializer.SerializeValue(ref _xPos);
            serializer.SerializeValue(ref _yPos);
            serializer.SerializeValue(ref _zPos);

            serializer.SerializeValue(ref _xRot);
            serializer.SerializeValue(ref _yRot);
            serializer.SerializeValue(ref _zRot);
        }
    }

    #region Interpolate State

    private Vector3 _posVel;
    private float _rotVelX;
    private float _rotVelY;
    private float _rotVelZ;

    private void ConsumeState()
    {
        transform.position = Vector3.SmoothDamp(transform.position, _playerState.Value.Position, ref _posVel, _cheapInterpolationTime);
        transform.rotation = Quaternion.Euler(
            Mathf.SmoothDampAngle(transform.rotation.eulerAngles.x, _playerState.Value.Rotation.x, ref _rotVelX, _cheapInterpolationTime),
            Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, _playerState.Value.Rotation.y, ref _rotVelY, _cheapInterpolationTime),
            Mathf.SmoothDampAngle(transform.rotation.eulerAngles.z, _playerState.Value.Rotation.z, ref _rotVelZ, _cheapInterpolationTime)
        );
    }

        #endregion
}