using EuNet.Rpc;
using System.Threading.Tasks;

namespace Common
{
    // Room Rpc Server to Client
    public interface IGameScRpc : IRpc
    {
        Task OnCreateRoom(RoomInfo roomInfo);
        Task OnJoinRoom(RoomInfo roomInfo);
        Task OnJoinRoomOtherUser(RoomSlotInfo roomSlotInfo);
        Task OnLeaveRoom(int roomId);
        Task OnLeaveRoomOtherUser(int roomId, byte slotId, ushort sessionId);
    }
}
