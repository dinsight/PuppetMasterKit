
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PuppetMasterKit.Terrain.Map
{
  /// <summary>
  /// interface for consuming 2D data
  /// </summary>
  interface I2DSubscript<T>
  {
    T this[int row, int col] { get; }
  }

  /// <summary>
  /// Region.
  /// </summary>
  public class Region : I2DSubscript<int?>
  {
    public int RegionFill { get; }
    public int MinRow { get; private set; } = int.MaxValue;
    public int MaxRow { get; private set; } = int.MinValue;
    public int MinCol { get; private set; } = int.MaxValue;
    public int MaxCol { get; private set; } = int.MinValue;

    public IReadOnlyCollection<GridCoord> Tiles {
      get {
        return new ReadOnlyCollection<GridCoord>(tiles.Keys.ToList());
      }
    }

    private Dictionary<GridCoord,int?> tiles = new Dictionary<GridCoord, int?>();

    /// <summary>
    /// Helper for consuming 2D arrays
    /// </summary>
    class ArraySubscript : I2DSubscript<int?>
    {
      readonly int[,] array;
      readonly int rows;
      readonly int cols;
      /// <summary>
      /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Terrain.Map.ArraySubscript"/> class.
      /// </summary>
      /// <param name="array">Array.</param>
      public ArraySubscript(int[,] array)
      {
        this.array = array;
        rows = array.GetLength(0);
        cols = array.GetLength(1);
      }
      public int? this[int row, int col] {
        get {
          if (row >= 0 && col >= 0 && row < rows && col < cols) {
            return array[row, col];
          }
          return null;
        }
      }
    }

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
      var key = new GridCoord(row, col);
      if (tiles.ContainsKey(key))
        return;

      tiles.Add(key, RegionFill);
      MinRow = Math.Min(MinRow, row);
      MaxRow = Math.Max(MaxRow, row);
      MinCol = Math.Min(MinCol, col);
      MaxCol = Math.Max(MaxCol, col);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="region"></param>
    public void Merge(Region region)
    {
      if (region == null)
        return;
      foreach (var item in region.Tiles) {
        AddTile(item.Row, item.Col);
      }
    }


    /// <summary>
    /// Gets the <see cref="T:PuppetMasterKit.AI.Region"/> with the specified row col.
    /// </summary>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    public int? this[int row, int col] {
      get {
        tiles.TryGetValue(new GridCoord(row, col), out int? value);
        return value;
      }
      set {
        var key = new GridCoord(row, col);
        if (tiles.ContainsKey(key)) {
          tiles[key] = value;
        }
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
      return ExtractRegions(new ArraySubscript(geography), rows, cols);
    }

    /// <summary>
    /// Extracts the regions.
    /// </summary>
    /// <returns>The regions.</returns>
    /// <param name="geography">Geography.</param>
    public static List<Region> ExtractRegions(Region geography)
    {
      var rows = geography.MaxRow + 1;
      var cols = geography.MaxCol + 1;
      return ExtractRegions(geography, rows, cols);
    }

    /// <summary>
    /// Extracts the regions.
    /// </summary>
    /// <returns>The regions.</returns>
    /// <param name="geography">Geography.</param>
    private static List<Region> ExtractRegions(I2DSubscript<int?> geography, int rows, int cols)
    {
      var regions = new List<Region>();
      var pr = new Region[cols];

      for (int row = 0; row < rows; row++) {
        var cr = new Region[cols];
        for (int col = 0; col < cols; col++) {
          Region reg = null;
          var val = geography[row, col];
          if (!val.HasValue)
            continue;
          var toMerge = pr.Where((x, i) => i >= col - 1
                              && i <= col + 1
                              && x != null && x.RegionFill == val);
          if (col > 0 && cr[col - 1] != null && cr[col - 1].RegionFill == val) {
            toMerge = toMerge.Concat(Enumerable.Repeat(cr[col - 1], 1));
          }

          toMerge = toMerge.Distinct();
          if (toMerge.Count() == 0) {
            reg = new Region(val.Value);
            regions.Add(reg);
          } else {
            reg = toMerge.First();
            toMerge.ForEach(x => {
              if (!Object.ReferenceEquals(x, reg)) {
                reg.Merge(x);
                regions.Remove(x);
              }
              for (int index = 0; index < cols; index++) {
                if (Object.ReferenceEquals(x, pr[index])) {
                  pr[index] = reg;
                }
                if (Object.ReferenceEquals(x, cr[index])) {
                  cr[index] = reg;
                }
              }
            });
          }
          cr[col] = reg;
          reg.AddTile(row, col);
        }
        pr = cr;
      }
      return regions;
    }

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
    /// Directions
    /// </summary>
    static readonly int N = 0, E = 1, S = 2, W = 3;
    //(row,col) coords of the neighbors to be visited from this position
    readonly int[,,] step = new int[4, 3, 2] {
        { { 1, 0}, { 0,-1}, { 1, 1} }, //n - Fwd, Left, Right
        { { 0, 1}, { 1, 0}, {-1, 1} }, //e
        { {-1, 0}, { 0, 1}, {-1,-1} }, //s
        { { 0,-1}, {-1, 0}, { 1,-1} }  //w
    };

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

      if (fwd == null && right != null) { // we have the wall on out right
        return new GridCoord(fwdRow, fwdCol);
      }
      if (fwd == null && right == null) { //turn right
        dir = (dir + 1) % 4; // change direction
        return new GridCoord(fwdRow, fwdCol);
      }

      if (fwd != null && left == null) { //change direction - turn left
        dir = dir - 1 >= 0 ? dir - 1 : W;
        return current;
      }
      if ((fwd != null && left != null && right != null) ||
          (fwd != null && left !=null && right == null)) { //change direction - turn back
        dir = (dir + 2) % 4;
        return current;
      }
      throw new Exception("Region.TraceContour: Get next should not return null");
    }

    /// <summary>
    /// Traverses the region.
    /// </summary>
    /// <param name="action">Action.</param>
    public void TraverseRegion(Action<int,int, TileType> action) 
    {
      var contour = TraceContour();
      Tiles.ForEach(a => action(a.Row, a.Col, TileType.Plain));

      for (int index = 0; index < contour.Count; index++) {

        var tileType = TileType.Plain;
        var c = contour[index];
        //get the prev and next tiles. Make sure to wrap around when the 
        //one of the ends of the list is reached
        var p = index == 0 ? contour[contour.Count - 1] : contour[index - 1];
        var n = index == contour.Count - 1 ? contour[0] : contour[index + 1];

        if (p.Col == c.Col && c.Col == n.Col && p.Row < c.Row && c.Row < n.Row) { //l
          tileType = TileType.LeftSide;
        }
        if (p.Col == c.Col && p.Row > c.Row && n.Col == c.Col && n.Row < c.Row) { //r
          tileType = TileType.RightSide;
        }
        if(p.Row == c.Row && p.Col < c.Col && n.Row == c.Row && n.Col > c.Col) { //t
          tileType = TileType.TopSide;
        }
        if (p.Row == c.Row && p.Col > c.Col && n.Row == c.Row && n.Col < c.Col) { //b
          tileType = TileType.BottomSide;
        }
        if (p.Col == c.Col && p.Row < c.Row && n.Row == c.Row && n.Col > c.Col) { //tlc
          tileType = TileType.TopLeftCorner;
        }
        if (p.Row == c.Row && p.Col < c.Col && n.Col == c.Col && n.Row < c.Row) { //trc
          tileType = TileType.TopRightCorner;
        }
        if (p.Row == c.Row && p.Col > c.Col && n.Col == c.Col && n.Row > c.Row) { //blc *
          tileType = TileType.BottomLeftCorner;
        }
        if (p.Col == c.Col && p.Row > c.Row && n.Row == c.Row && n.Col < c.Col) { //brc
          tileType = TileType.BottomRightCorner;
        }
        if (p.Col == c.Col && p.Row < c.Row && n.Row == c.Row && n.Col < c.Col) { //blj
          tileType = TileType.BottomLeftJoint;
        }
        if (p.Row == c.Row && p.Col > c.Col && n.Col == c.Col && n.Row < c.Row) { //brj
          tileType = TileType.BottomRightJoint;
        }
        if (p.Col == c.Col && p.Row > c.Row && n.Row == c.Row && n.Col > c.Col) { //trj
          tileType = TileType.TopRightJoint;
        }
        if (p.Row == c.Row && p.Col < c.Col && n.Col == c.Col && n.Row > c.Row) { //tlj
          tileType = TileType.TopLeftJoint;
        }
        action(c.Row, c.Col, tileType);
      }
    }
  }
}