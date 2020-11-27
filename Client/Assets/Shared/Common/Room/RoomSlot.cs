using EuNet.Core;
using System;

namespace Common
{
    public abstract class RoomSlot : IRoomSlotUserInfo
    {
        public readonly byte Id;

        public long UserId { get; protected set; }
        public string Name { get; protected set; }

        public ISession Session { get; protected set; }
        public IRoomListner Listner { get; protected set; }

        public bool IsEmpty => UserId == 0 ? true : false;
        public bool IsSessionEmpty => (UserId == 0 || Session == null) ? true : false;

        public RoomSlot(byte slotId)
        {
            Id = slotId;
        }

        public void Clear()
        {
            UserId = 0;
            Name = string.Empty;
            Session = null;
            Listner = null;
        }

        public bool SetUser(
            UserInfo userInfo,
            ISession session,
            IRoomListner listner)
        {
            if (IsEmpty == false)
                return false;

            UserId = userInfo.Id;
            Name = userInfo.Name;

            Session = session;
            Listner = listner;
            return true;
        }

        public RoomSlotInfo AsRoomSlotInfo()
        {
            return new RoomSlotInfo()
            {
                SlotId = Id,
                UserId = UserId,
                Name = Name,
                SessionId = IsEmpty == false && Session != null ? Session.SessionId : (ushort)0,
            };
        }
    }
}
