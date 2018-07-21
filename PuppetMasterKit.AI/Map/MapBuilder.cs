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
    /// Randomize the specified count, rangeStart and rangeEnd.
    /// </summary>
    /// <returns>The randomize.</returns>
    /// <param name="count">Count.</param>
    /// <param name="rangeStart">Range start.</param>
    /// <param name="rangeEnd">Range end.</param>
    private List<int> Randomize(int count, int rangeStart, int rangeEnd)
    {
      List<int> numbers = new List<int>();
      Random r = new Random(Guid.NewGuid().GetHashCode());
      for (int i = 0; i < count; ++i) {
        numbers.Add(r.Next(rangeStart, rangeEnd));
      }
      return numbers;
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
        rooms.Add(room);
        room.Id = rooms.Count - 1;
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

    /// <summary>
    /// Creates the paths.
    /// </summary>
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
