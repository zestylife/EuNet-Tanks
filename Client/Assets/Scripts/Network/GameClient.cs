using Common;
using CommonResolvers;
using EuNet.Core;
using EuNet.Unity;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class GameClient : Singleton<GameClient>
{
    private NetClientP2pBehaviour _client;

    // Room
    public ClientRoom Room { get; private set; }

    // Rpcs
    public GameCsRpc Rpc { get; private set; }

    public UserInfo UserInfo;
    public NetClientP2p Client => _client.ClientP2p;

    protected override void Awake()
    {
        base.Awake();
        if (IsDestroyed)
            return;

        _client = GetComponent<NetClientP2pBehaviour>();
        _client.SetClientOptionFunc = (clientOption) =>
        {
            clientOption.PacketFilter = new XorPacketFilter(536651);
        };

        CustomResolver.Register(GeneratedResolver.Instance);
    }

    private void Start()
    {
        Client.OnConnected = OnConnected;
        Client.OnClosed = OnClosed;
        Client.OnReceived = OnReceive;
        Client.OnP2pGroupLeaved = OnP2pGroupLeave;

        // 자동으로 생성된 Rpc 서비스를 사용하기 위해 등록함
        Client.AddRpcService(new GameClientService());
        Client.AddRpcService(new ActorRpcServiceView());
        Client.AddRpcService(new GameManagerRpcServiceView());
    }

    private void OnConnected()
    {
        Rpc = new GameCsRpc(_client.Client, null, TimeSpan.FromSeconds(10));
    }

    private void OnClosed()
    {
        GameManager.Instance?.OnDisconnect();
    }

    private Task OnReceive(NetDataReader reader)
    {
        return Task.CompletedTask;
    }

    private void OnP2pGroupLeave(ushort sessionId, bool isMine)
    {
        
    }

    public void SetRoom(RoomInfo roomInfo)
    {
        Room = new ClientRoom(roomInfo);
    }

    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            Client.Disconnect();
        }
    }
}
