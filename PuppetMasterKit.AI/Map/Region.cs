using PuppetMasterKit.AI.Map;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PuppetMasterKit.AI
{
  public class Region
  {
    public int RegionFill { get; }

    public IReadOnlyCollection<GridCoord> Tiles {
      get {
        return new ReadOnlyCollection<GridCoord>(tiles.Values.ToList());
      }
    }

    public int MinRow { get => minRow; }
    public int MaxRow { get => maxRow; }
    public int MinCol { get => minCol; }
    public int MaxCol { get => maxCol; }

    private Dictionary<int, GridCoord> tiles = new Dictionary<int, GridCoord>();
    private int minRow = int.MaxValue;
    private int maxRow = int.MinValue;
    private int minCol = int.MaxValue;
    private int maxCol = int.MinValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Region"/> class.
    /// </summary>
    /// <param name="regionFill">Region fill.</param>
    public Region(int regionFill)
    {
      this.RegionFill = regionFill;
    }

    /// <summary>
    /// Hashs the func.
    /// </summary>
    /// <returns>The func.</returns>
    /// <param name="a">The alpha component.</param>
    /// <param name="b">The blue component.</param>
    private int HashFunc(int a, int b)
    {
      var hashCode = 1084646500;
      hashCode = hashCode * -1521134295 + a.GetHashCode();
      hashCode = hashCode * -1521134295 + b.GetHashCode();
      return hashCode;
    }

    /// <summary>
    /// Adds the tile.
    /// </summary>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    public void AddTile(int row, int col)
    {
      tiles.Add(HashFunc(row, col), new GridCoord(row, col));
      minRow = Math.Min(MinRow, row);
      maxRow = Math.Max(MaxRow, row);
      minCol = Math.Min(MinCol, col);
      maxCol = Math.Max(MaxCol, col);
    }

    /// <summary>
    /// Gets the <see cref="T:PuppetMasterKit.AI.Region"/> with the specified row col.
    /// </summary>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    public GridCoord this[int row, int col] {
      get {
        GridCoord value = null;
        tiles.TryGetValue(HashFunc(row, col), out value);
        return value;
      }
    }

    /// <summary>
    /// Extracts the regions.
    /// </summary>
    /// <returns>The regions.</returns>
    /// <param name="geography">Geography.</param>
    public static List<Region> ExtractRegions(int[,] geography)
    {
      var rows = geography.GetLength(0);
      var cols = geography.GetLength(1);

      var dictRegions = new Dictionary<int, Region>();
      for (int row = 0; row < rows; row++) {
        for (int col = 0; col < cols; col++) {
          var val = geography[row, col];
          Region region = null;
          if (dictRegions.ContainsKey(val)) {
            region = dictRegions[val];
          } else {
            region = new Region(val);
            dictRegions.Add(val, region);
          }
          region.AddTile(row, col);
        }
      }
      return dictRegions.Values.ToList();
    }

    private static readonly int E = 0;
    private static readonly int S = 1;
    private static readonly int W = 2;
    private static readonly int N = 3;

    //(row,col) coords of the neighbors to be visited fro m this position
    readonly int[,,] step = new int[4, 3, 2] {
        { { 1, 1}, { 0, 1}, {-1, 1} },
        { {-1, 1}, {-1, 0}, {-1,-1} },
        { {-1,-1}, { 0,-1}, { 1,-1} },
        { { 1,-1}, { 1, 0}, { 1, 1} }
      };

    /// <summary>
    /// Returns a list of tiles wrapping the current region
    /// </summary>
    /// <returns>The contour.</returns>
    public List<GridCoord> TraceContour()
    {
      var dir = N;
      var result = new List<GridCoord>();
      var min = this.Tiles.MinBy(x => x.Col);
      var start = new GridCoord(min.Row, min.Col - 1);
      result.Add(start);
      var next = GetNext(start, ref dir);
      while (next != start) {
        result.Add(next);
        next = GetNext(next, ref dir);
      }
      return result;
    }

    /// <summary>
    /// Gets the next tile in the direction specified
    /// </summary>
    /// <returns>The next.</returns>
    /// <param name="current">Current.</param>
    /// <param name="dir">Dir.</param>
    private GridCoord GetNext(GridCoord current, ref int dir)
    {
      var i = 0;
      var spin = 0;
      int prevRow = 0, prevCol = 0;
      // if all "next" positions turn out to be null, rotate right (dir++)
      //to prevent spinning around indefinitly, limit the number of turns to 4 (N,E,S,W)
      while (spin < 4) {
        for (i = 0; i < 3; i++) {
          var row = current.Row + step[dir, i, 0];
          var col = current.Col + step[dir, i, 1];
          var tile = this[row, col];
          if (tile != null)
            return new GridCoord(prevRow, prevCol);
          prevCol = col;
          prevRow = row;
        }
        dir = (dir + 1) % 4;
        spin++;
      }
      return null;
    }
  }
}
