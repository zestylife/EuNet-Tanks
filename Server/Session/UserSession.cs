using EuNet.Core;
using EuNet.Server;
using Common;
using System;
using System.Threading.Tasks;

namespace GameServer
{
    public partial class UserSession : ServerSession
    {
        private UserInfo _userInfo;
        private GameScRpc _rpc;

        public static long s_autoIncrementUserId = 1;

        public UserSession(SessionCreateInfo createInfo)
            : base(createInfo)
        {
            _rpc = new GameScRpc(this);
        }

        protected override void OnConnected()
        {
            base.OnConnected();

            _userInfo = null;
        }

        protected override Task OnClosed()
        {
            if (_userInfo != null)
            {
                if(_roomId > 0)
                {
                    // 접속해제시 방에서 떠남
                    Rooms.Instance.Leave(_roomId, _userInfo.Id);
                }
            }

            return base.OnClosed();
        }

        public override void OnError(Exception ex)
        {
            base.OnError(ex);
            Console.Error.WriteLine(ex.ToString());
        }
    }
}
