using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class BlockNetworkTransform : NetworkBehaviour
{
    // private readonly NetworkVariable<BlockNetworkState> _blockState = new(writePerm: NetworkVariableWritePermission.Owner);

    private Rigidbody rb;

    [SerializeField] private float _cheapInterpolationTime = 0.1f;

    // toggle for client/server authority
    [SerializeField] private bool usingServerAuth;
    // network variables only changeable by server
    private NetworkVariable<BlockNetworkState> _blockState;

    private void Awake()
    {
        var permission = usingServerAuth ? NetworkVariableWritePermission.Server : NetworkVariableWritePermission.Owner;
        _blockState = new NetworkVariable<BlockNetworkState>(writePerm: permission);

        rb = GetComponent<Rigidbody>();

    }

    // public override void OnNetworkSpawn() 
    // {
    //     base.OnNetworkSpawn();

    //     if (IsClient)
    //     {
    //         ConsumeState();
    //     }
    // }

    void Update()
    {

        if (!NetworkManager.IsConnectedClient) return;

        // either writing or reading from network
        if (IsOwner)
        {
            if (!rb.IsSleeping())
            {
                // print("block transmitting");
                TransmitState();
            }
            else {
                print("nope");
            }
        }
        else { 
            ConsumeState(); 
        }
    }

    #region Transmit State

    private void TransmitState() {
        BlockNetworkState state = new BlockNetworkState{
            AtRest = rb.IsSleeping(),
            Position = transform.position,
            Rotation = transform.rotation.eulerAngles,
        };


        // if script not set to using server as authority
        // then just write our value
        // if the block is the server itself, then also can execute immediately
        if (IsServer || !usingServerAuth) {
            _blockState.Value = state;
        }
        else {
            // but if server authority is on, client cannot write to variable
            // need to tell server "this is my state, tell this to rest of the clients"
            // RPC = remote procedure call = code that can be run on another system
            TransmitStateServerRpc(state);
            
        }
    }

    [ServerRpc]
    // when this runs on server, update block state
    private void TransmitStateServerRpc(BlockNetworkState state) {
        _blockState.Value = state;
    }

    #endregion



    struct BlockNetworkState : INetworkSerializable
    {
        // dont want to modify directly
        private bool atRest;
        private float _xPos, _yPos, _zPos;
        private short _xRot, _yRot, _zRot;

        internal bool AtRest
        {
            get => atRest;
            set { atRest = value; }
        }

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

        // How to serialize
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T: IReaderWriter
        {
            serializer.SerializeValue(ref atRest);
            if (!atRest)
            {
                // print("block transmitting");
            
                serializer.SerializeValue(ref _xPos);
                serializer.SerializeValue(ref _yPos);
                serializer.SerializeValue(ref _zPos);

                serializer.SerializeValue(ref _xRot);
                serializer.SerializeValue(ref _yRot);
                serializer.SerializeValue(ref _zRot);
            }
        }
    }

    #region Interpolate State

    private Vector3 _posVel;
    private float _rotVelX;
    private float _rotVelY;
    private float _rotVelZ;

    private void ConsumeState()
    {
        transform.position = Vector3.SmoothDamp(transform.position, _blockState.Value.Position, ref _posVel, _cheapInterpolationTime);
        transform.rotation = Quaternion.Euler(
            Mathf.SmoothDampAngle(transform.rotation.eulerAngles.x, _blockState.Value.Rotation.x, ref _rotVelX, _cheapInterpolationTime),
            Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, _blockState.Value.Rotation.y, ref _rotVelY, _cheapInterpolationTime),
            Mathf.SmoothDampAngle(transform.rotation.eulerAngles.z, _blockState.Value.Rotation.z, ref _rotVelZ, _cheapInterpolationTime)
        );
        // print("Local Scale: " + transform.localScale.y);
        // print("blockstate Scale: " + _blockState.Value.Scale.y);
    }

    #endregion
}