using Common;
using EuNet.Client;
using EuNet.Unity;
using UnityEngine;

public class ClientRoomSlot : RoomSlot
{
    public P2pMember Member { get; protected set; }

    public ClientRoomSlot(byte slotId)
        : base(slotId)
    {
        
    }

    public void Set(RoomSlotInfo info)
    {
        var member = NetClientGlobal.Instance.Client.P2pGroup.Find(info.SessionId);

        Member = member;
        Session = member != null ? member.Session : null;
        
        UserId = info.UserId;
        Name = info.Name;
    }
}
