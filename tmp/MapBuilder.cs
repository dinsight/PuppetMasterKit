using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2.Map
{
    public class MapBuilder
    {
        public static readonly int Blank = int.MinValue;

        public int Rows { get; private set; }
        public int Cols { get; private set; }

        private int[,] map;
        private int roomPadding;
        private List<Room> rooms = new List<Room>();
        private IPathFinder pathFinder;
        
        public MapBuilder(int rows, int cols, int roomPadding, IPathFinder pathFinder)
        {
            this.pathFinder = pathFinder;
            this.Rows = rows;
            this.Cols = cols;
            this.roomPadding = roomPadding;
            map = new int[Rows, Cols];
            ResetMap();
        }

        private void ResetMap()
        {
            for (int i = 0; i < Rows; i++){
                for (int j = 0; j < Cols; j++){
                    map[i, j] = Blank;
                }
            }
        }
        private bool CanAdd(Module module, int row, int col)
        {
            var toAdd = module;
            if (roomPadding > 0) {
                toAdd = module.Pad(roomPadding);
            }
            return toAdd.CanFit(map, row, col);
        }

        private List<int> Randomize(int count, int rangeStart, int rangeEnd)
        {
            List<int> numbers = new List<int>();
            Random r = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < count; ++i)
            {
                numbers.Add(r.Next(rangeStart, rangeEnd));
            }
            return numbers;
        }

        public Room AddRoom(Module module, int row, int col)
        {
            if(CanAdd(module, row, col)) {
                module.Stamp(map, row, col);
                var room = new Room(module, row, col);
                rooms.Add(room);
                return room;
            }   
            return null;
        }

        public void Apply(Action<int,int,int> action)
        {
            for (int i = 0; i < Rows; i++){
                for (int j = 0; j < Cols; j++){
                    action(i,j,map[i, j]);
                }
            }
        }

        public int Create(int maxRooms, List<Module> modules) {
            var rooms = Randomize(maxRooms, 0, modules.Count);
            var rows = Randomize(maxRooms, 0, Rows);
            var cols = Randomize(maxRooms, 0, Cols);
            var actual = 0;
            for (int i = 0; i < rooms.Count; i++){
                if (null != AddRoom(modules[rooms[i]], rows[i], cols[i])) {
                    actual++;
                }
            }
            CreatePaths();
            return actual;
        }

        private void CreatePaths() {
            for (int index = 0; index < rooms.Count; index++) {
                var room = rooms[index];
                for (int j = index + 1; j < rooms.Count; j++)
                {
                    var nextRoom = rooms[j];
                    var path = pathFinder.Find(map, room.Row, room.Col, nextRoom.Row, nextRoom.Col);
                    path.ForEach(x => map[x.Item1, x.Item2] = MapCodes.PATH);
                }
            }
        }
    }
}
