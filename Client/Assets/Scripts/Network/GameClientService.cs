using Common;
using EuNet.Unity;
using System.Threading.Tasks;
using UnityEngine;

public class GameClientService : GameScRpcServiceAbstract
{
    public override Task OnCreateRoom(RoomInfo roomInfo)
    {
        GameClient.Instance.SetRoom(roomInfo);

        return Task.CompletedTask;
    }

    public override Task OnJoinRoom(RoomInfo roomInfo)
    {
        GameClient.Instance.SetRoom(roomInfo);

        return Task.CompletedTask;
    }

    public override Task OnJoinRoomOtherUser(RoomSlotInfo roomSlotInfo)
    {
        GameClient.Instance.Room.OnJoinRoomOtherUser(roomSlotInfo);

        return Task.CompletedTask;
    }

    public override Task OnLeaveRoom(int roomId)
    {
        Debug.Log("OnLeaveRoom");
        return Task.CompletedTask;
    }

    public override Task OnLeaveRoomOtherUser(int roomId, byte slotId, ushort sessionId)
    {
        Debug.Log($"OnLeaveRoomOtherUser {roomId} {slotId} {sessionId}");

        var room = GameClient.Instance.Room;

        if(room.Id == roomId)
        {
            room.OnLeaveRoomOtherUser(slotId);

            // 다른유저가 방에서 떠나면
            // 해당 유저의 액터를 삭제한다
            ActorManager.Instance.ActorList.ForEach((Actor actor) =>
            {
                if (actor.View.OwnerSessionId == sessionId)
                {
                    Debug.Log($"OnLeaveRoomOtherUser Destroy {sessionId} {actor.Nickname}");
                    NetClientGlobal.Instance.Destroy(actor.View.ViewId);
                }
            });
        }

        return Task.CompletedTask;
    }
}
