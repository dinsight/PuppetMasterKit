
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PuppetMasterKit.Terrain.Map
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

    static readonly int N = 0, E = 1, S = 2, W = 3;
    //(row,col) coords of the neighbors to be visited fro m this position
    readonly int[,,] step = new int[4, 3, 2] {
        { { 1, 0}, { 0,-1}, { 1, 1} }, //n - Fwd, Left, Right
        { { 0, 1}, { 1, 0}, {-1, 1} }, //e
        { {-1, 0}, { 0, 1}, {-1,-1} }, //s
        { { 0,-1}, {-1, 0}, { 1,-1} }  //w
      };

    /// <summary>
    /// Returns a list of tiles wrapping the current region
    /// The algorithm consists in walking around the tiles always
    /// touching the wall with your right hand. If there is no tile on
    /// your right, turn right. If there is a tile in front, turn left.
    /// Repeat until you reach the first tile
    /// </summary>
    /// <returns>The contour.</returns>
    public List<GridCoord> TraceContour()
    {
      int dir = N;//North
      var result = new List<GridCoord>();
      var min = this.Tiles.MinBy(x => x.Col);
      var start = new GridCoord(min.Row, min.Col - 1);
      result.Add(start);
      var next = GetNext(start, ref dir);
      while (next != start) {
        if (result.Last() != next) {
          result.Add(next);
        }
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
      var fwdRow = current.Row + step[dir, 0, 0];
      var fwdCol = current.Col + step[dir, 0, 1];
      var leftRow = current.Row + step[dir, 1, 0];
      var leftCol = current.Col + step[dir, 1, 1];
      var rightRow = current.Row + step[dir, 2, 0];
      var rightCol = current.Col + step[dir, 2, 1];

      var fwd = this[fwdRow, fwdCol];
      var left = this[leftRow, leftCol];
      var right = this[rightRow, rightCol];

      if(fwd == null && right != null){ // we have the wall on out right
        return new GridCoord(fwdRow, fwdCol);
      }
      if(fwd == null && right == null) { //turn right
        dir = (dir + 1) % 4; // change direction
        return new GridCoord(fwdRow, fwdCol);
      }

      if(fwd != null && left == null){ //change direction - turn left
        dir = dir - 1 >= 0 ? dir - 1 : W;
        return current;
      }
      if (fwd != null && left != null && right != null) { //change direction - turn back
        dir = (dir + 2) % 4;
        return current;
      }
      throw new Exception("Region.TraceContour: Get next should not return null");
    }
  }
}
