namespace Common
{
    public interface IRoomListner
    {
        void OnCreateRoom(Room room, RoomSlot slot);
        void OnJoinRoom(Room room, RoomSlot slot);
        void OnJoinRoomOtherUser(Room room, RoomSlot slot);
        void OnLeaveRoom(Room room, byte slotId, ushort sessionId);
        void OnLeaveRoomOtherUser(Room room, byte slotId, ushort sessionId);
    }
}
