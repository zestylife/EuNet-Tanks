using Common;
using EuNet.Core;

namespace GameServer
{
    public partial class UserSession : IRoomListner
    {
        private int _roomId;
        public int RoomId { get { return _roomId; } set { _roomId = value; } }
        
        public ISession Session => this;

        public long Id => _userInfo.Id;
        public string Name => _userInfo.Name;

        public void OnCreateRoom(Room room, RoomSlot slot)
        {
            RoomId = room.Id;
            _rpc.WithNoReply().OnCreateRoom(room.AsRoomInfo());
        }

        public void OnJoinRoom(Room room, RoomSlot slot)
        {
            // 본인이 입장
            RoomId = room.Id;
            _rpc.WithNoReply().OnJoinRoom(room.AsRoomInfo());
        }

        public void OnJoinRoomOtherUser(Room room, RoomSlot slot)
        {
            // 다른유저가 입장
            _rpc.WithNoReply().OnJoinRoomOtherUser(slot.AsRoomSlotInfo());
        }

        public void OnLeaveRoom(Room room, byte slotId, ushort sessionId)
        {
            if (State != SessionState.Connected)
                return;

            // 본인이 나감
            RoomId = 0;
            _rpc.WithNoReply().OnLeaveRoom(room.Id);
        }

        public void OnLeaveRoomOtherUser(Room room, byte slotId, ushort sessionId)
        {
            // 다른유저가 나감
            _rpc.WithNoReply().OnLeaveRoomOtherUser(room.Id, slotId, sessionId);
        }
    }
}
