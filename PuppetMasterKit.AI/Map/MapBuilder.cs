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
    private IPathFinder pathFinder;
    private List<Room> rooms = new List<Room>();
    private Dictionary<Module, Module> paddedModules = new Dictionary<Module, Module>();

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
      var paddedModule = module;
      if (roomPadding > 0) {
        if (paddedModules.ContainsKey(module)) {
          paddedModule = paddedModules[module];
        } else {
          paddedModule = module.Pad(roomPadding);
          paddedModules.Add(module, paddedModule);
        }
      }
      return paddedModule.CanFit(map, row, col);
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
        var room = new Room(module, map, row, col);
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
      ParitionMap();
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
      TieDeadends(connected);
    }

    /// <summary>
    /// Ties the deadends.
    /// </summary>
    /// <returns>The deadends.</returns>
    /// <param name="connected">Connected.</param>
    private void TieDeadends(List<Room> connected)
    {
      var deadends = rooms.Where(x => x.PathCount <= 1).ToList();
      var pairs = GetClosestPair(deadends, deadends, roomDistance);
      while (pairs != null) {
        if (pairs.Item1.PathCount == 1
            && pairs.Item2.PathCount == 1
            && CreatePath(pairs.Item1, pairs.Item2, false, false)) {
          pathCount++;
        }
        deadends.Remove(pairs.Item1);
        deadends.Remove(pairs.Item2);
        pairs = GetClosestPair(deadends, deadends, roomDistance);
      };
    }

    /// <summary>
    /// Voronoi-like partitioning of the map
    /// </summary>
    public void ParitionMap()
    {
      Apply((r, c, v) => {
      if (v == Blank) {
          var closestRoom = rooms.MinBy(x => Point.Distance(r,c,x.Row,x.Col));
          map[r, c] = MapCodes.REGION + closestRoom.Id;
        }
      });
    }

    /// <summary>
    /// Creates the path.
    /// </summary>
    /// <returns><c>true</c>, if path was created, <c>false</c> otherwise.</returns>
    /// <param name="room">Room.</param>
    /// <param name="nextRoom">Next room.</param>
    public bool CreatePath(Room room, Room nextRoom,
                           bool canCrossOtherPaths = true,
                           bool crossOtherRooms = true)
    {
      var path = pathFinder.Find(map, room.Row, room.Col, nextRoom.Row, nextRoom.Col);
      if (!canCrossOtherPaths) {
        var isCrossingOtherPaths = path.Any(x => map[x.Item1, x.Item2] == MapCodes.PATH);
        if (isCrossingOtherPaths)
          return false;
      }
      if (!crossOtherRooms) {
        var countCrossingOtherRooms = path.Where(x =>
            map[x.Item1, x.Item2] == MapCodes.X &&
            !room.IsInside(x.Item1, x.Item2) &&
            !nextRoom.IsInside(x.Item1, x.Item2)).Count();
        if (countCrossingOtherRooms > 0)
          return false;
      }
      if (path.Count > 0) {
        path.ForEach(x => {
          if (map[x.Item1, x.Item2] == MapBuilder.Blank) {
            map[x.Item1, x.Item2] = MapCodes.PATH;
          }
        });
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
