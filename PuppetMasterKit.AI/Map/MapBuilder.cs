using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.AI.Map;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.AI
{
  public class MapBuilder
    {
        public static readonly int Blank = int.MinValue;

        public int Rows { get; private set; }
        public int Cols { get; private set; }

        private int[,] map;
        private int roomPadding;
        private int pathCount;
        private List<Room> rooms = new List<Room>();
        private IPathFinder pathFinder;
        
        public MapBuilder(int rows, int cols, int roomPadding, IPathFinder pathFinder)
        {
            this.pathFinder = pathFinder;
            this.Rows = rows;
            this.Cols = cols;
            this.pathCount = 0;
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
                room.Id = rooms.Count-1;
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

        internal void CreatePaths()
        {
            pathCount = 0;
            var connected = new List<Room>();
            var unconnected = new Queue<Room>(rooms.Skip(1));
            connected.Add(rooms.First());

            while (unconnected.Count > 0) {
                var newRoom = unconnected.Dequeue();
                var sorted = connected
                    .OrderBy(x => Point.Distance(x.Row, x.Col, newRoom.Row, newRoom.Col));
                var pathFound = false;
                foreach (var item in sorted) {
                    if (CreatePath(item, newRoom)) {
                        pathFound = true;
                        break;
                    }
                }
                if (pathFound) {
                    pathCount++;
                    connected.Add(newRoom);
                } else {
                    ;
                }
            }
        }

        internal bool CreatePath(Room room, Room nextRoom)
        {
            var nextRoomExits = nextRoom.GetExits(map).Select(x => new Point(x.Item1, x.Item2));
            var roomExits = room.GetExits(map).Select(x => new Point(x.Item1, x.Item2));
            var exitPair = GetClosestPair(roomExits, nextRoomExits, (a,b)=>Point.Distance(a,b));
            var path = pathFinder.Find(map,
                (int)exitPair.Item1.X,
                (int)exitPair.Item1.Y,
                (int)exitPair.Item2.X,
                (int)exitPair.Item2.Y);
            if (path.Count > 0) {
                path.ForEach(x => map[x.Item1, x.Item2] = MapCodes.PATH + pathCount);
                return true;
            }
            return false;
        }

        private Tuple<T, T> GetClosestPair<T>(
            IEnumerable<T> set1, 
            IEnumerable<T> set2,
            Func<T,T,float> distance)
        {
            float minDist = float.MaxValue;
            T start = default(T);
            T end = default(T);
            foreach (var s in set1) {
                foreach (var d in set2) {
                    var dist = distance(s, d); 
                    if (dist < minDist){
                        minDist = dist;
                        start = s;
                        end = d;
                    }
                }
            }
            if (start != null && end != null)
                return Tuple.Create(start, end);
            return null;
        }
    }
}

/*
private static void PrintMap(MapBuilder builder)
        {
            var pathCh = 'A'- MapCodes.PATH;
            var buffer = new StringBuilder();
            var line = new StringBuilder();
            line.Append("    ");
            for (int i = 0; i < cols; i++){
                line.Append((i % 10).ToString("D1"));
            }
            buffer.Append(line.ToString());
            buffer.AppendLine();
            line.Length = 0;
            line.Append("000 ");
            builder.Apply((i, j, x) =>{
                if (x == MapBuilder.Blank) {
                    line.Append("*");
                } else if (x >= MapCodes.PATH) {
                    var c = (char)(pathCh + x);
                    line.Append(c);
                } else {
                    line.Append(x.ToString());
                }
                if (j == cols - 1){
                    line.AppendLine();
                    buffer.Append(line.ToString());
                    line.Length = 0;
                    line.Append((i + 1).ToString("D3") + " ");
                }
            });
            Console.WriteLine(buffer.ToString());
            Console.ReadKey();
        }
        
static void Main(string[] args)
        {
            var C = MapCodes.CENTER;
            var E = MapCodes.EXIT;
            var builder = new MapBuilder(rows, cols, 2, new PathFinder() );
            var modules = new List<Module>();

            var module1 = new Module(new int[,] {
                { 0,0,1,1,E,1,1,0,0,0 },
                { 0,1,1,1,1,1,1,1,1,0 },
                { 1,1,1,1,1,1,1,1,1,1 },
                { 0,1,1,1,1,C,1,1,1,0 },
                { 1,1,1,1,1,1,1,1,1,0 },
                { 0,1,1,1,1,1,1,1,1,0 },
                { 0,0,1,1,E,1,1,0,0,1 },
            });

            var module2 = new Module(new int[,] {
                { 0,0,0,0,E,0,0,0,0,0 },
                { 1,1,1,1,1,1,1,1,1,1 },
                { 1,1,1,1,1,1,1,1,1,1 },
                { 1,1,1,1,1,C,1,1,1,1 },
                { 1,1,1,1,1,1,1,1,1,1 },
                { 1,1,1,1,1,1,1,1,1,1 },
                { 1,1,1,1,1,1,1,E,1,1 },
            });

            modules.Add(module1);
            //var roomCount = 300;
            //var actual = builder.Create(roomCount, modules);
            //Console.WriteLine($"Created {actual} out of {roomCount}");
            var r1= builder.AddRoom(module1, 20, 10);
            var r2 = builder.AddRoom(module1, 30, 35);
            var r3 = builder.AddRoom(module1, 15, 45);
            var r4 = builder.AddRoom(module1, 40, 25);
            var r5 = builder.AddRoom(module1, 20, 65);
            builder.CreatePaths();
            //builder.CreatePath(r2, r3);

            PrintMap(builder);
        }        
*/
