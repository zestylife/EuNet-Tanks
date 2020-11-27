using EuNet.Core;
using EuNet.Server;
using Common;
using System;
using CommonResolvers;

namespace GameServer
{
    public class Server
    {
        private NetServer _server;
        public NetServer NetServer => _server;
        public P2pManager P2pManager => _server.P2pManager;

        private static Server _instance;
        public static Server Instance => _instance;

        public Server()
        {
            _instance = this;

            // 자동으로 생성된 리졸버를 등록해줌
            CustomResolver.Register(GeneratedResolver.Instance);
        }

        public void Start()
        {
            // 서버 옵션을 정의
            var serverOption = new ServerOption()
            {
                Name = "Server",
                TcpServerPort = 12000,
                IsServiceUdp = true,
                UdpServerPort = 12001,
                MaxSession = 1000,
                SessionUpdateInternval = 10,
                IsCheckAlive = true,
                CheckAliveInterval = 20000,
                CheckAliveTimeout = 30000,
                PacketFilter = new XorPacketFilter(536651)
            };

            // 로거 팩토리를 생성
            var loggerFactory = DefaultLoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddConsoleLogger();
            });

            var statistics = new NetStatistic();

            // UserSession 을 사용하기 위해서 팩토리를 만듬
            var sessionFactory = new DefaultSessionFactory(
                serverOption,
                loggerFactory,
                statistics,
                createInfo => {
                    return new UserSession(createInfo);
                });

            // 서버를 생성
            _server = new NetServer(
                serverOption,
                statistics,
                loggerFactory,
                sessionFactory);

            // 자동으로 생성된 Rpc 서비스를 등록함
            _server.AddRpcService(new GameCsRpcServiceSession());

            // 룸들을 생성
            new Rooms(100);

            // 서버를 시작함
            _server.StartAsync().Wait();

            // 메인스레드에 키 입력을 받을 수 있게 함
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape)
                {
                    Console.WriteLine("quit");
                    break;
                }
            }

            // 서버 정지
            _server.StopAsync().Wait();
        }
    }
}
