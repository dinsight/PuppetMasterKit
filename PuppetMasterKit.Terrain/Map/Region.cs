
using PuppetMasterKit.Utility.Subscript;
using PuppetMasterKit.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.Terrain.Map
{
  /// <summary>
  /// Region.
  /// </summary>
  public class Region : I2DSubscript<int?>
  {
    public enum RegionType
    {
      REGION,
      PATH
    }

    public RegionType Type { get; set; }
    public int RegionFill { get; set; }
    public int MinRow { get; private set; } = int.MaxValue;
    public int MaxRow { get; private set; } = int.MinValue;
    public int MinCol { get; private set; } = int.MaxValue;
    public int MaxCol { get; private set; } = int.MinValue;
    public int Rows => this.MaxRow - this.MinRow + 1;
    public int Cols => this.MaxCol - this.MinCol + 1;

    public IReadOnlyCollection<GridCoord> Tiles
    {
      get {
        return new ReadOnlyCollection<GridCoord>(tiles.Keys.ToList());
      }
    }

    private Dictionary<GridCoord, int?> tiles = new Dictionary<GridCoord, int?>();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Region"/> class.
    /// </summary>
    /// <param name="regionFill">Region fill.</param>
    public Region(int regionFill) {
      this.RegionFill = regionFill;
      this.Type = RegionType.REGION;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Terrain.Map.Region"/> class.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="regionFill">Region fill.</param>
    public Region(RegionType type, int regionFill = 0) {
      this.RegionFill = regionFill;
      this.Type = type;
    }

    /// <summary>
    /// Hashs the func.
    /// </summary>
    /// <returns>The func.</returns>
    /// <param name="a">The alpha component.</param>
    /// <param name="b">The blue component.</param>
    private int HashFunc(int a, int b) {
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
    public void AddTile(int row, int col) {
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
    public void Merge(Region region) {
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
    public int? this[int row, int col]
    {
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
    public static List<Region> ExtractRegions(int[,] geography) {
      var rows = geography.GetLength(0);
      var cols = geography.GetLength(1);
      return ExtractRegions(new ArraySubscript<int>(geography), rows, cols);
    }

    /// <summary>
    /// Extracts the regions.
    /// </summary>
    /// <returns>The regions.</returns>
    /// <param name="geography">Geography.</param>
    public static List<Region> ExtractRegions(Region geography) {
      var rows = geography.MaxRow + 1;
      var cols = geography.MaxCol + 1;
      return ExtractRegions(geography, rows, cols);
    }

    /// <summary>
    /// Extracts the regions.
    /// </summary>
    /// <returns>The regions.</returns>
    /// <param name="geography">Geography.</param>
    private static List<Region> ExtractRegions(I2DSubscript<int?> geography, int rows, int cols) {
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
    /// 
    /// </summary>
    /// <returns></returns>
    public List<Contour> TraceInsideContour() {
      return TraceContour(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public List<Contour> TraceOutsideContour()
    {
      return TraceContour(true);
    }

    /// <summary>
    /// Returns a list of tiles wrapping the current region
    /// The algorithm consists in walking around the tiles always
    /// touching the wall with your right hand (or left hand if we trace the inside contour). 
    /// If there is no tile on your right (left), turn right (left). If there is a tile in front, turn left (right).
    /// Repeat until you reach the first tile
    /// </summary>
    /// <returns>The contour.</returns>
    private List<Contour> TraceContour(bool traceOutsideContour) {
      var result = new List<Contour>();
      //trace outer contour
      var min = this.Tiles.MinBy(x => x.Col);
      var start = traceOutsideContour ? new GridCoord(min.Row, min.Col - 1) : 
                                        new GridCoord(min.Row, min.Col);
      //because we start from the leftmost tile, we set the start direction to North
      result.Add(TraceContourFrom(start, traceOutsideContour, N));

      //trace inner contours
      //multiple contours can occur if the region is not contiguos and it is split in "isles" 
      var visited = new Dictionary<GridCoord, bool>();
      for (int row = MinRow; row <= MaxRow; row++) {
        int? col = ScanRow(row, MinCol, null);
        while (col != null && (col = ScanRow(row, col.Value, RegionFill)) != null) {
          var cv = this[row, col.Value];
          var next = ScanRow(row, col.Value, cv);
          if(next<MaxCol){ 
            var sp = traceOutsideContour ? new GridCoord(row, col.Value) : 
                                         new GridCoord(row, col.Value - 1);
            if (!visited.ContainsKey(sp)) {
              //we start tracing from the rightmost tile, so we need start 
              //moving downward (South)
              var contour = TraceContourFrom(sp, traceOutsideContour, S);
              contour.ContourLines.ForEach(x => {
                if (!visited.ContainsKey(x))
                  visited.Add(x, true);
              });
              result.Add(contour);
            }
          }
          col = next;
        }
      }
      return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="traceOutsideContour"></param>
    /// <param name="startDirection">N,S,E,W</param>
    /// <returns></returns>
    private Contour TraceContourFrom(GridCoord start, 
      bool traceOutsideContour, 
      int startDirection) 
    {
      int dir = startDirection;
      var result = new List<GridCoord> { start };
      var next = GetNext(start, traceOutsideContour, ref dir);
      while (next != start) {
        if (result.Last() != next) {
          result.Add(next);
        }
        next = GetNext(next, traceOutsideContour, ref dir);
      }
      var contourType = traceOutsideContour ?
          Contour.ContourType.OUTSIDE
        : Contour.ContourType.INSIDE;
      return new Contour(contourType, result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private int? ScanRow(int row, int col, int? value) {
      int jj = col;
      //scan the line while the value is "value"
      for (; jj <= MaxCol && this[row, jj] == value; jj++);
      //stop at the end of the row
      if (jj > MaxCol)
        return null;
      //or when we have a value different than "value"
      return jj;
    }

    /// <summary>
    /// Directions
    /// </summary>
    static readonly int N = 0, E = 1, S = 2, W = 3;
    //(row,col) coords of the neighbors to be visited from this position
    readonly int[,,] step = new int[4, 3, 2] {
        { { 1, 0}, { 0,-1}, { 0, 1} }, //n - Fwd, Left, Right
        { { 0, 1}, { 1, 0}, {-1, 0} }, //e
        { {-1, 0}, { 0, 1}, { 0,-1} }, //s
        { { 0,-1}, {-1, 0}, { 1, 0} }  //w
    };

    /// <summary>
    /// Gets the next tile in the direction specified
    /// </summary>
    /// <returns>The next.</returns>
    /// <param name="current">Current.</param>
    /// <param name="dir">Dir.</param>
    private GridCoord GetNext(GridCoord current, bool traceOutsideContour, ref int dir) {
      var fwdRow = current.Row + step[dir, 0, 0];
      var fwdCol = current.Col + step[dir, 0, 1];
      var leftRow = current.Row + step[dir, 1, 0];
      var leftCol = current.Col + step[dir, 1, 1];
      var rightRow = current.Row + step[dir, 2, 0];
      var rightCol = current.Col + step[dir, 2, 1];

      var fwd   = this[fwdRow, fwdCol] != null;
      var left  = this[leftRow, leftCol] != null;
      var right = this[rightRow, rightCol] != null;

      if (traceOutsideContour) { // OUT SIDE CONTOUR 
        if (!fwd && right) { // we have the wall on out right
          return new GridCoord(fwdRow, fwdCol);
        }
        if (!right) { //turn right
          dir = (dir + 1) % 4; // change direction
          return new GridCoord(rightRow, rightCol);
        }
        if (fwd && !left) { //change direction - turn left
          dir = dir - 1 >= 0 ? dir - 1 : W;
          return new GridCoord(leftRow, leftCol);
        }
        if (fwd && left) { //cul de sac -change direction - turn back
          dir = (dir + 2) % 4;
          return current;
        }
      } else { //INSIDE CONTOUR (the "wall" in this case are the *EMPTY* tiles)
        if (fwd && !left) { // we have the wall on our left
          return new GridCoord(fwdRow, fwdCol);
        }

        if (left) { //turn left
          dir = dir - 1 >= 0 ? dir - 1 : W;
          return new GridCoord(leftRow, leftCol);
        }

        if (!fwd && right) { //change direction - turn right
          dir = (dir + 1) % 4; // change direction
          return new GridCoord(rightRow, rightCol);
        }
        if (!fwd && !right) { //cul de sac -change direction - turn back
          dir = (dir + 2) % 4;
          return current;
        }
      }
      throw new Exception("Region.TraceContour: Get next should not return null");
    }

    /// <summary>
    /// Traverses the region.
    /// </summary>
    /// <param name="action">Action.</param>
    public void TraverseRegion(Action<int, int, TileType> action, bool traceOutsideContour = true) {
      var contours = TraceContour(traceOutsideContour);
      var allContourTiles = contours.SelectMany(x => x.ContourLines).ToList();

      if (traceOutsideContour) {
        Tiles.ForEach(a => action(a.Row, a.Col, TileType.Plain));
      } else {
        Tiles.Except(allContourTiles).OrderBy(x=>x.Row).ThenBy(x=>x.Col)
          .ForEach(a => action(a.Row, a.Col, TileType.Plain));
      }

      contours.ForEach(contour => {
        var chain = contour.ContourLines.ToList();
        TraverseChain(chain, action);
      });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsChain() {
      GridCoord prev = null;
      foreach (var tile in Tiles) {
        if (prev != null) {
          if (!tile.IsAdjacentTo(prev)){
            return false;
          }
        }
        prev = tile;
      }
      return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chain"></param>
    /// <param name="action"></param>
    public void TraverseChain(List<GridCoord> chain, Action<int, int, TileType> action)
    {
      for(var index = 0; index < chain.Count;index++) {
        var tileType = TileType.Unknown;
        //get the prev and next tiles. 
        var c = chain[index];
        var p = index == 0 ? null : chain[index - 1];
        var n = index == chain.Count - 1 ? null : chain[index + 1];

        //prev and next nodes do not exists. Assume a RightSide type of tile
        if (p == null && n == null) {
          action(c.Row, c.Col, TileType.RightSide);
          return;
        }

        //there's no previous tile. If the last tile in the chaine is adjacent to
        //it, set it as prev. Otherwise, create a dummy tile so we can have a direction
        //for the first tile
        if (p == null) {
          var last = chain[chain.Count - 1];
          if (c.IsAdjacentTo(last)) {
            p = last;
          } else {
            p = new GridCoord(c.Row - (n.Row-c.Row), c.Col - (n.Col-c.Col));
          }
        }
        //there is no next tile. If the current tile is ajacent to the first one,
        //choose the first as "next". Otherwise create dummy tile to give a direction
        if (n == null) {
          var first = chain[0];
          if (c.IsAdjacentTo(first)) {
            n = first;
          } else {
            n = new GridCoord(c.Row + (c.Row-p.Row), c.Col + (c.Col-p.Col));
          }
        }
        tileType = DetermineTileType(tileType, c, p, n);
        action(c.Row, c.Col, tileType);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="c"></param>
    /// <param name="p"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    private static TileType DetermineTileType(TileType tileType, GridCoord c, GridCoord p, GridCoord n)
    {
      if (p.Col == c.Col && c.Col == n.Col && p.Row < c.Row && c.Row < n.Row) { //l
        tileType = TileType.LeftSide;
      }
      if (p.Col == c.Col && p.Row > c.Row && n.Col == c.Col && n.Row < c.Row) { //r
        tileType = TileType.RightSide;
      }
      if (p.Row == c.Row && p.Col < c.Col && n.Row == c.Row && n.Col > c.Col) { //t
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
      if (p.Row == n.Row && p.Col == n.Col && c.Col > p.Col && c.Row == p.Row) { //cdsl
        tileType = TileType.CulDeSacLeft;
      }
      if (p.Row == n.Row && p.Col == n.Col && c.Col < p.Col && c.Row == p.Row) { //cdsr
        tileType = TileType.CulDeSacRight;
      }
      if (p.Row == n.Row && p.Col == n.Col && c.Row < p.Row && c.Col == p.Col) { //cdst
        tileType = TileType.CulDeSacTop;
      }
      if (p.Row == n.Row && p.Col == n.Col && c.Row > p.Row && c.Col == p.Col) { //cdsb
        tileType = TileType.CulDeSacBottom;
      }
      return tileType;
    }
  }
}