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
    public List<Room> Rooms { get => rooms; }

    private int[,] map;
    private int roomPadding;
    private int pathCount;
    private List<Room> rooms = new List<Room>();
    private IPathFinder pathFinder;
    private float roomDistance(Room a, Room b) => Point.Distance(a.Row, a.Col, b.Row, b.Col);

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.MapBuilder"/> class.
    /// </summary>
    /// <param name="rows">Rows.</param>
    /// <param name="cols">Cols.</param>
    /// <param name="roomPadding">Room padding.</param>
    /// <param name="pathFinder">Path finder.</param>
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

    /// <summary>
    /// Resets the map.
    /// </summary>
    private void ResetMap()
    {
      for (int i = 0; i < Rows; i++) {
        for (int j = 0; j < Cols; j++) {
          map[i, j] = Blank;
        }
      }
    }

    /// <summary>
    /// Cans the add.
    /// </summary>
    /// <returns><c>true</c>, if add was caned, <c>false</c> otherwise.</returns>
    /// <param name="module">Module.</param>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    private bool CanAdd(Module module, int row, int col)
    {
      var toAdd = module;
      if (roomPadding > 0) {
        toAdd = module.Pad(roomPadding);
      }
      return toAdd.CanFit(map, row, col);
    }

    /// <summary>
    /// Adds the room.
    /// </summary>
    /// <returns>The room.</returns>
    /// <param name="module">Module.</param>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    public Room AddRoom(Module module, int row, int col)
    {
      if (CanAdd(module, row, col)) {
        module.Stamp(map, row, col);
        var room = new Room(module, row, col);
        Rooms.Add(room);
        room.Id = Rooms.Count - 1;
        return room;
      }
      return null;
    }

    /// <summary>
    /// Apply the specified action.
    /// </summary>
    /// <param name="action">Action.</param>
    public void Apply(Action<int, int, int> action)
    {
      for (int i = 0; i < Rows; i++) {
        for (int j = 0; j < Cols; j++) {
          action(i, j, map[i, j]);
        }
      }
    }

    /// <summary>
    /// Create the specified maxRooms and modules.
    /// </summary>
    /// <returns>The create.</returns>
    /// <param name="maxRooms">Max rooms.</param>
    /// <param name="modules">Modules.</param>
    public int Create(int maxRooms, List<Module> modules)
    {
      var actual = 0;
      int index = 0;
      Random randomRoom = new Random(Guid.NewGuid().GetHashCode());
      Random randomCoord = new Random(Guid.NewGuid().GetHashCode());
      while (index++ < maxRooms) {
        var roomId = randomRoom.Next(0, modules.Count);
        var coord = randomCoord.Next(0, Rows * Cols);
        var rowId = coord / Cols;
        var colId = coord % Cols;
        if (null != AddRoom(modules[roomId], rowId, colId)) {
          actual++;
        }
      }
      CreatePaths();
      return actual;
    }

    /// <summary>
    /// Creates the paths.
    /// </summary>
    public void CreatePaths()
    {
      pathCount = 0;
      var connected = new List<Room>();
      var unconnected = new List<Room>(Rooms.Skip(1));
      connected.Add(Rooms.First());

      var closest = GetClosestPair(connected, unconnected, roomDistance);
      while (closest != null) {
        if (CreatePath(closest.Item1, closest.Item2)) {
          pathCount++;
          connected.Add(closest.Item2);
          unconnected.Remove(closest.Item2);
        }
        closest = GetClosestPair(connected, unconnected, roomDistance);
      }
      //TieDeadends(connected);
    }

    /// <summary>
    /// Ties the deadends.
    /// </summary>
    /// <returns>The deadends.</returns>
    /// <param name="connected">Connected.</param>
    private void TieDeadends(List<Room> connected)
    {
      var deadends = rooms.Where(x => x.PathCount <= 1).ToList();
      var pairs = ( from x in deadends
                    from y in deadends
                    where //roomDistance(x, y) < Math.Max(Rows,Cols) / 2 
                        //&& 
                        !Object.ReferenceEquals(x, y)
                    select new { x,y } ).ToList();
      do {
        var p = pairs.First();
        if (p.x.PathCount==1 && p.y.PathCount==1 && CreatePath(p.x, p.y)) {
          pathCount++;
        }
        pairs.Remove(p);
      } while (pairs.Count() > 0);
    }
    /// <summary>
    /// Creates the path.
    /// </summary>
    /// <returns><c>true</c>, if path was created, <c>false</c> otherwise.</returns>
    /// <param name="room">Room.</param>
    /// <param name="nextRoom">Next room.</param>
    public bool CreatePath(Room room, Room nextRoom)
    {
      var exitPair = Tuple.Create(new Point(room.Row, room.Col),
                                  new Point(nextRoom.Row, nextRoom.Col));
      if (exitPair == null)
        return false;
      var path = pathFinder.Find(map,
          (int)exitPair.Item1.X,
          (int)exitPair.Item1.Y,
          (int)exitPair.Item2.X,
          (int)exitPair.Item2.Y);
      if (path.Count > 0) {
        path.ForEach(x => map[x.Item1, x.Item2] = MapCodes.PATH + pathCount);
        room.PathCount++;
        nextRoom.PathCount++;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Gets the closest pair.
    /// </summary>
    /// <returns>The closest pair.</returns>
    /// <param name="set1">Set1.</param>
    /// <param name="set2">Set2.</param>
    /// <param name="distance">Distance.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    private Tuple<T, T> GetClosestPair<T>(
        IEnumerable<T> set1,
        IEnumerable<T> set2,
        Func<T, T, float> distance) where T : class
    {
      float minDist = float.MaxValue;
      T start = default(T);
      T end = default(T);
      foreach (var s in set1) {
        foreach (var d in set2) {
          if (Object.ReferenceEquals(s, d))
            continue;
          var dist = distance(s, d);
          if (dist < minDist) {
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
