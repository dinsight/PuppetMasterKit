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
      for (int i = 0; i < Rows; i++) {
        for (int j = 0; j < Cols; j++) {
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
      for (int i = 0; i < count; ++i) {
        numbers.Add(r.Next(rangeStart, rangeEnd));
      }
      return numbers;
    }

    public Room AddRoom(Module module, int row, int col)
    {
      if (CanAdd(module, row, col)) {
        module.Stamp(map, row, col);
        var room = new Room(module, row, col);
        rooms.Add(room);
        return room;
      }
      return null;
    }

    public void Apply(Action<int, int, int> action)
    {
      for (int i = 0; i < Rows; i++) {
        for (int j = 0; j < Cols; j++) {
          action(i, j, map[i, j]);
        }
      }
    }

    public int Create(int maxRooms, List<Module> modules)
    {
      var roomIds = Randomize(maxRooms, 0, modules.Count);
      var rowIds = Randomize(maxRooms, 0, Rows);
      var colIds = Randomize(maxRooms, 0, Cols);
      var actual = 0;
      for (int i = 0; i < roomIds.Count; i++) {
        if (null != AddRoom(modules[roomIds[i]], rowIds[i], colIds[i])) {
          actual++;
        }
      }
      CreatePaths();
      return actual;
    }

    private void CreatePaths()
    {
     var connected = new List<Tuple<int,int>>();
     for (int index = 0; index < rooms.Count; index++) {
         var room = rooms[index];
         var nextRoom = rooms.Where(r =>!(room.Row == r.Row && room.Col == r.Col))
             .MinBy(r => Point.Distance(room.Row, room.Col, r.Row, r.Col)) ;

         if (connected.FirstOrDefault(x => x.Item1 == room.Id && x.Item2 == nextRoom.Id) == null)
         {
             connected.Add(Tuple.Create(room.Id, nextRoom.Id));
             var sources = room.GetExits(map).Select(x => new Point(x.Item1, x.Item2));
             var destinations = nextRoom.GetExits(map).Select(x => new Point(x.Item1, x.Item2));
             float minDist = float.MaxValue;
             var start = (Point)null;
             var end= (Point)null;
             foreach (var s in sources){
                 foreach (var d in destinations){
                     var dist = Point.Distance(s, d);
                     if (dist < minDist) {
                         minDist = dist;
                         start = s;
                         end = d;
                     }
                 }
             }
             
             var path = pathFinder.Find(map, (int)start.X, (int)start.Y, (int)end.X, (int)end.Y);
             path.ForEach(x => map[x.Item1, x.Item2] = MapCodes.PATH);
         }
     }
    }
  }
}
