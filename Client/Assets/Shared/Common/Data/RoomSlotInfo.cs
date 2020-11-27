using EuNet.Core;

namespace Common
{
    [NetDataObject]
    public class RoomSlotInfo
    {
        // 슬롯 아이디
        public byte SlotId;

        // 세션 아이디 (접속해있지 않으면 0)
        public ushort SessionId;

        // 유저 고유아이디
        public long UserId;

        // 유저 이름
        public string Name;
    }
}
