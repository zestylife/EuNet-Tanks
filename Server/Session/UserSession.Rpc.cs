using Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer
{
    public partial class UserSession : IGameCsRpc
    {
        public Task<int> Login(int version, string id, string authKey)
        {
            if(version != 1)
                throw new Exception($"Not match version");

            if (authKey != "key")
                throw new Exception("Not match authKey");

            return Task.FromResult(0);
        }

        public Task<UserInfo> GetUserInfo(string name)
        {
            _userInfo = new UserInfo();
            _userInfo.Id = Interlocked.Increment(ref s_autoIncrementUserId);
            _userInfo.Name = name;

            return Task.FromResult(_userInfo);
        }

        public Task<bool> QuickJoinRoom(GameMode gameMode)
        {
            if (RoomId > 0)
                return Task.FromResult(false);

            var joinRoomInfo = new JoinRoomInfo()
            {
                UserInfo = _userInfo,
                Session = this,
                Listner = this,
                GameMode = gameMode
            };

            var result = Rooms.Instance.QuickJoinRoom(ref joinRoomInfo);

            return Task.FromResult(result);
        }
    }
}
