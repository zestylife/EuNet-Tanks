using Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClientRoom : Room
{
    public ClientRoom(RoomInfo roomInfo)
        : base(roomInfo.Id, slotId => new ClientRoomSlot(slotId))
    {
        _gameMode = roomInfo.GameMode;
        
        for(int i=0; i<_slots.Count; ++i)
        {
            var slot = _slots[i] as ClientRoomSlot;
            slot.Set(roomInfo.Slots[i]);
        }
    }

    public IEnumerable<T> GetFilledSlots<T>() where T : RoomSlot
    {
        return _slots.Where(x => x.UserId != 0).Select(x => x as T).Where(x => x != null);
    }

    public ClientRoomSlot Find(byte slotId)
    {
        return _slots[slotId] as ClientRoomSlot;
    }

    public ClientRoomSlot FindBySessionId(ushort sessionId)
    {
        return _slots
            .Where(x => !x.IsEmpty && x.Session != null && x.Session.SessionId == sessionId)
            .Select(x => x as ClientRoomSlot)
            .FirstOrDefault();
    }

    public void OnJoinRoomOtherUser(RoomSlotInfo roomSlotInfo)
    {
        var slot = Find(roomSlotInfo.SlotId);
        slot?.Set(roomSlotInfo);
    }

    public void OnLeaveRoomOtherUser(byte slotId)
    {
        var slot = Find(slotId);
        slot?.Clear();
    }
}
