using EuNet.Core;
using System.Collections.Generic;

namespace Common
{
    [NetDataObject]
    public class RoomInfo
    {
        public int Id;
        public GameMode GameMode;
        public List<RoomSlotInfo> Slots;
    }
}
