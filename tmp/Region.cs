using PuppetMasterKit.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
//using Pair = System.Tuple<int, int>;

namespace PuppetMasterKit.AI
{

  public class GridCoord
  {
    public int Row { get; private set; }
    public int Col { get; private set; }
    public GridCoord(int row, int col) {
      Row = row;
      Col = col;
    }
    public static bool operator==(GridCoord lhs, GridCoord rhs){ 
      if(Object.ReferenceEquals(lhs,null) && Object.ReferenceEquals(rhs,null) ){ 
        return true;
        }
     if(Object.ReferenceEquals(lhs,null) || Object.ReferenceEquals(rhs,null) )
        return false;
      return lhs.Row == rhs.Row && lhs.Col == rhs.Col;
    }
    public static bool operator!=(GridCoord lhs, GridCoord rhs){ 
      return !(lhs==rhs);
    }
  }

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
    /// https://en.wikipedia.org/wiki/Pairing_function#Cantor_pairing_function
    /// </summary>
    /// <returns></returns>
    /// <param name="">.</param>
    private int HashFunc(int a, int b)
    {
      return $"{a}{b}".GetHashCode();
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


    static int E = 0, S = 1, W = 2, N = 3;
    //(row,col) coords of the neighbors to be visited fro m this position
    int[,,] step = new int[4,3,2] { 
        { { 1, 1}, { 0, 1}, {-1, 1} },
        { {-1, 1}, {-1, 0}, {-1,-1} },
        { {-1,-1}, { 0,-1}, { 1,-1} },
        { { 1,-1}, { 1, 0}, { 1, 1} }
      };

    public List<GridCoord> TraceContour(){ 
      var dir = N;
      var result = new List<GridCoord>();
      var min = this.Tiles.MinBy(x=>x.Col);
      var start = new GridCoord(min.Row, min.Col-1);
      result.Add(start);
      var next = GetNext(start, ref dir);
      while(next != start){ 
        result.Add(next);
        next = GetNext(next, ref dir);
      }
      return result;
    }

    private GridCoord GetNext(GridCoord current, ref int dir) {
      var i = 0;
      var spin = 0;
      int prevRow = 0, prevCol = 0;
      // if all "next" positions turn out to be null, rotate right (dir++)
      //to prevent spinning around indefinitly, limit the number of turns to 4 (N,E,S,W)
      while(spin<4){ 
        for (i = 0; i < 3; i++) {
          var row = current.Row+step[dir,i,0];
          var col = current.Col+step[dir,i,1];
          var tile = this[row, col];
          if(tile!=null)
            return new GridCoord(prevRow, prevCol);
          prevCol = col;
          prevRow = row;
        }
        dir = (dir+1)%4; 
        spin++;
      }
      return null;
    }
  }
}
