using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public ref struct JoinRoomInfo
    {
        public UserInfo UserInfo;
        public UserSession Session;
        public IRoomListner Listner;
        public GameMode GameMode;
    }
}
