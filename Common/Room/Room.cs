using EuNet.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public abstract class Room
    {
        public readonly int Id;

        protected List<RoomSlot> _slots;
        protected GameMode _gameMode;

        public GameMode GameMode => _gameMode;
        public bool IsEmpty => !_slots.Where(x => !x.IsEmpty).Any();
        public bool IsFull => !_slots.Where(x => x.IsEmpty).Any();
        public bool IsAny => _slots.Where(x => !x.IsEmpty).Any();

        public const int MaxRoomUser = 10;

        public Room(int id, Func<byte, RoomSlot> slotCreater)
        {
            Id = id;
            _slots = new List<RoomSlot>(MaxRoomUser);
            for (int i = 0; i < MaxRoomUser; ++i)
                _slots.Add(slotCreater((byte)i));
        }

        public IEnumerable<T> GetSessions<T>(ISession except = null) where T : class, ISession
        {
            lock (_slots)
            {
                return _slots.Where(x => x.Session != null && x.Session != except).Select(x => x.Session as T).Where(x => x != null);
            }
        }

        public RoomInfo AsRoomInfo()
        {
            lock (_slots)
            {
                return new RoomInfo()
                {
                    Id = Id,
                    GameMode = _gameMode,
                    Slots = new List<RoomSlotInfo>(_slots.Select(x => x.AsRoomSlotInfo()))
                };
            }
        }
    }
}
