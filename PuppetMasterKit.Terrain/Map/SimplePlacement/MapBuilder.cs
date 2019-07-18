using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.Graphics.Geometry;
using System.Collections.ObjectModel;
using PuppetMasterKit.Utility.Extensions;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.Terrain.Map.SimplePlacement
{
  public class MapBuilder : IMapGenerator
  {
    public static readonly int Blank = int.MinValue;

    private int[,] map;
    private readonly int maxRooms;
    private readonly List<Module> modules;
    private readonly int roomPadding;
    private int pathCount;
    private IPathFinder pathFinder;
    private List<Room> rooms = new List<Room>();
    private List<Region> regions = new List<Region>();
    private Dictionary<Module, Module> paddedModules = new Dictionary<Module, Module>();
    private float RoomDistance(Room a, Room b) => Point.Distance(a.Row, a.Col, b.Row, b.Col);

    public int Rows { get; private set; }
    public int Cols { get; private set; }
    public IReadOnlyCollection<Room> Rooms { get => new ReadOnlyCollection<Room>(rooms); }
    public IReadOnlyCollection<Region> Regions { get => new ReadOnlyCollection<Region>(regions); }
    public int[,] Map { get => map; }

    public int? this[int row, int col] => map[row,col];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="maxRooms"></param>
    /// <param name="modules"></param>
    /// <param name="roomPadding"></param>
    /// <param name="pathFinder"></param>
    public MapBuilder(int maxRooms, List<Module> modules, int roomPadding, IPathFinder pathFinder) {
      this.pathFinder = pathFinder;
      this.pathCount = 0;
      this.maxRooms = maxRooms;
      this.modules = modules;
      this.roomPadding = roomPadding;
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
        rooms.Add(room);
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
    /// 
    /// </summary>
    /// <returns></returns>
    private int Create()
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
      regions = CreateRegions();
      CreatePaths();
      return actual;
    }

    /// <summary>
    /// Creates the paths.
    /// </summary>
    public void CreatePaths()
    {
      pathCount = 0;
      var accessible = Rooms.Where(room => room.Module.IsAccessible);
      var connected = new List<Room>();
      //need at least two rooms to connect
      if (accessible.Count() < 2)
        return;
      //skip the first room and add it to the connected list
      var unconnected = new List<Room>(accessible.Skip(1));
      connected.Add(accessible.First());

      //get the closest pair of rooms and build a path between them
      var closest = GetClosestPair(connected, unconnected, RoomDistance);
      while (closest != null) {
        var path = CreatePath(closest.Item1, closest.Item2);
        if (path != null) {
          regions.Add(path);
          pathCount++;
          connected.Add(closest.Item2);
          unconnected.Remove(closest.Item2);
        }
        closest = GetClosestPair(connected, unconnected, RoomDistance);
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
      var deadends = rooms.Where(x => x.Module.IsAccessible && x.PathCount <= 1).ToList();
      var pairs = GetClosestPair(deadends, deadends, RoomDistance);
      while (pairs != null) {
        if (pairs.Item1.PathCount == 1 && pairs.Item2.PathCount == 1) {
          var path = CreatePath(pairs.Item1, pairs.Item2, false, false);
          if (path != null) {
            regions.Add(path);
            pathCount++;
          }
        }
        deadends.Remove(pairs.Item1);
        deadends.Remove(pairs.Item2);
        pairs = GetClosestPair(deadends, deadends, RoomDistance);
      };
    }

    /// <summary>
    /// Voronoi-like partitioning of the map
    /// </summary>
    public List<Region> CreateRegions()
    {
      Apply((r, c, v) => {
        if (v == Blank) {
          //var closestRoom = rooms.MinBy(x => Point.Distance(r, c, x.Row, x.Col));
          var closestRoom = rooms.MinBy(x => Math.Abs(r - x.Row) + Math.Abs(c - x.Col));
          map[r, c] = closestRoom.Module.RegionFill;
        }
      });
      return Region.ExtractRegions(map);  
    }

    /// <summary>
    /// Creates a path between two rooms.
    /// </summary>
    /// <returns><c>true</c>, if path was created, <c>false</c> otherwise.</returns>
    /// <param name="room">Room.</param>
    /// <param name="nextRoom">Next room.</param>
    public Region CreatePath(Room room, Room nextRoom,
                           bool canCrossOtherPaths = true,
                           bool crossOtherRooms = true)
    {
      var result = new List<GridCoord>();
      var path = pathFinder.Find(map, room.Row, room.Col, nextRoom.Row, nextRoom.Col);
      if (!canCrossOtherPaths) {
        var isCrossingOtherPaths = path.Any(x => map[x.Item1, x.Item2] == MapCodes.PATH);
        if (isCrossingOtherPaths)
          return null;
      }
      if (!crossOtherRooms) {
        var countCrossingOtherRooms = path.Count(x =>
            map[x.Item1, x.Item2] == MapCodes.X &&
            !room.IsInside(x.Item1, x.Item2) &&
            !nextRoom.IsInside(x.Item1, x.Item2));
        if (countCrossingOtherRooms > 0)
          return null;
      }
      if (path.Count > 0) {
        path.ForEach(x => {
          if (map[x.Item1, x.Item2] == MapBuilder.Blank) {
            map[x.Item1, x.Item2] = MapCodes.PATH;
          }
        });
        room.PathCount++;
        nextRoom.PathCount++;
        var pathRegion = new Region(Region.RegionType.PATH);
        path.ForEach(x => {
          pathRegion.AddTile(x.Item1, x.Item2);
          pathRegion[x.Item1, x.Item2] = map[x.Item2, x.Item1];
        });
        return pathRegion;
      }
      return null;
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rows"></param>
    /// <param name="cols"></param>
    /// <returns></returns>
    public List<Region> Create(int rows, int cols)
    {
      this.Rows = rows;
      this.Cols = cols;
      map = new int[Rows, Cols];
      ResetMap();
      Create();
      return regions;
    }
    
    public void UpdateFrom(Region region) {
      throw new NotImplementedException();
    }
  }
}
