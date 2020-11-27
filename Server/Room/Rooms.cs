using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class Rooms
    {
        private readonly int MaxCount;
        private List<ServerRoom> _rooms;

        private static Rooms _instance;
        public static Rooms Instance => _instance;

        public Rooms(int maxCount)
        {
            _instance = this;

            MaxCount = maxCount;
            _rooms = new List<ServerRoom>(MaxCount);

            for (int i=0; i<MaxCount; i++)
            {
                // 룸 아이디는 인덱스+1
                var roomId = i + 1;
                _rooms.Add(new ServerRoom(roomId));
            }
        }

        public bool QuickJoinRoom(ref JoinRoomInfo joinInfo)
        {
            // 불변의 리스트이므로 동기화가 필요없음
            foreach(var room in _rooms)
            {
                if (room.GameMode != joinInfo.GameMode)
                    continue;

                if(room.Join(ref joinInfo))
                    return true;
            }

            // 입장을 못했으므로 생성한다
            return CreateRoom(ref joinInfo);
        }

        public bool CreateRoom(ref JoinRoomInfo joinInfo)
        {
            foreach (var room in _rooms)
            {
                if (room.Create(ref joinInfo))
                    return true;
            }

            return false;
        }

        public bool Leave(int roomId, long userId)
        {
            var room = Find(roomId);
            if (room == null)
                return false;

            return room.Leave(userId);
        }

        public ServerRoom Find(int roomId)
        {
            if (roomId <= 0 || roomId > MaxCount)
                return null;

            return _rooms[roomId - 1];
        }
    }
}
