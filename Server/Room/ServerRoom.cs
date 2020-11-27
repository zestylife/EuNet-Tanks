using Common;
using EuNet.Core;
using EuNet.Server;
using System;
using System.Linq;

namespace GameServer
{
    public class ServerRoom : Room
    {
        private P2pGroup _p2pGroup;

        public ServerRoom(int id)
            : base(id, (slotId) => { return new ServerRoomSlot(slotId); })
        {
            _p2pGroup = Server.Instance.P2pManager.CreateP2pGroup();
        }

        public bool Create(ref JoinRoomInfo createInfo)
        {
            // 동기화 전에 검색한다
            if (IsAny)
                return false;

            lock (_slots)
            {
                // 동기화 후에 확실히 한번 더 검색한다
                if (IsAny)
                    return false;

                var slot = _slots[0];
                if (slot.IsEmpty)
                {
                    _gameMode = createInfo.GameMode;

                    if (JoinInternal(ref createInfo, slot, true))
                        return true;
                }
            }

            return false;
        }

        public bool Join(ref JoinRoomInfo joinInfo)
        {
            lock (_slots)
            {
                if (IsEmpty)
                    return false;

                foreach (var slot in _slots)
                {
                    // 빈슬롯을 찾는다
                    if (slot.IsEmpty)
                    {
                        if (JoinInternal(ref joinInfo, slot, false))
                            return true;
                    }
                }
            }

            return false;
        }

        private bool JoinInternal(ref JoinRoomInfo joinInfo, RoomSlot slot, bool isCreate)
        {
            // P2p 그룹에 조인한다
            if (_p2pGroup.Join(joinInfo.Session))
            {
                // 방에 유저를 셋팅한다
                slot.SetUser(
                    joinInfo.UserInfo,
                    joinInfo.Session,
                    joinInfo.Listner);

                // 유저에게 이벤트 호출
                if (isCreate)
                {
                    joinInfo.Listner.OnCreateRoom(this, slot);
                }
                else
                {
                    joinInfo.Listner.OnJoinRoom(this, slot);

                    var joinSession = joinInfo.Session;

                    foreach (var findSlot in _slots.Where(x => !x.IsEmpty && x.Session != joinSession))
                    {
                        findSlot.Listner?.OnJoinRoomOtherUser(this, slot);
                    }
                }

                return true;
            }

            return false;
        }

        public bool Leave(long userId)
        {
            lock (_slots)
            {
                foreach (var slot in _slots)
                {
                    // 유저 삭제
                    if (slot.UserId == userId)
                    {
                        var sessionId = slot.Session.SessionId;

                        // P2p그룹을 떠난다
                        _p2pGroup.Leave(slot.Session as UserSession);

                        // 유저에게 이벤트 호출
                        slot.Listner.OnLeaveRoom(this, slot.Id, sessionId);

                        foreach (var findSlot in _slots.Where(x => !x.IsEmpty && x != slot))
                        {
                            findSlot.Listner.OnLeaveRoomOtherUser(this, slot.Id, sessionId);
                        }

                        slot.Clear();

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
