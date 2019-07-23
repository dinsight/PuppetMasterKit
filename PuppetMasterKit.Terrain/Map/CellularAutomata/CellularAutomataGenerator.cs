using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.Utility.Extensions;
using System;

namespace PuppetMasterKit.Terrain.Map.CellularAutomata
{
  public class CellularAutomataGenerator : IMapGenerator
  {
    public const int OFF = 0;
    private const int ON = 1;
    private readonly int generations;
    private int bornThreshold;
    private int surviveThreshold;
    private readonly Func<int, int, int , int, int> seed;
    private int[,] map;

    public int Rows { get; private set; }
    public int Cols { get; private set; }
    public int? this[int row, int col] => map[row,col];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="generations"></param>
    /// <param name="seed"></param>
    public CellularAutomataGenerator(int generations,
      int bornThreshold, int surviveThreshold,
      Func<int,int,int, int, int> seed)
    {
      this.generations = generations;
      this.bornThreshold = bornThreshold;
      this.surviveThreshold = surviveThreshold;
      this.seed = seed;
      this.Rows = 0;
      this.Cols = 0;
      this.map = new int[0, 0];
    }

    /// <summary>
    /// 
    /// </summary>
    private void Initialize() {
      for (int i = 0; i < Rows; i++) {
        for (int j = 0; j < Cols; j++) {
          map[i, j] = seed(i, j, Rows, Cols);
        }
      }
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
      var prev = new int[rows, cols];
      var current = new int[rows, cols];
      map = prev;
      Initialize();

      for (int i = 0; i < this.generations; i++) {
        Generate(i, prev, current);
        var temp = prev;
        prev = current;
        current = temp;
      }
      map = prev;
      PostProcess(map);
      return RemoveSmallRegions(Region.ExtractRegions(map));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentGeneration"></param>
    /// <param name="genPrev"></param>
    /// <param name="gen"></param>
    private void Generate(int currentGeneration, int[,] genPrev, int[,] gen)
    {
      //Set a post-processing threshold at about 80% of the steps
      var postStepPct = (int)(0.9*generations);

      for (int i = 0; i < Rows; i++) {
        for (int j = 0; j < Cols; j++) {
          var val = genPrev[i, j];
          //if(i==10 && j==54 && currentGeneration==generations-1){ 
          //  var tmp = genPrev[i, j];
          //}
          var live = GetNeighbours(genPrev, x => x > 0, i, j).Count();
          var liveStep2 = GetNeighbours(genPrev, x => x > 0, i, j, 2).Count();
          
          gen[i, j] = genPrev[i, j];
          if(IsTooNarrow(genPrev, i, j) && currentGeneration >= postStepPct){ 
            gen[i, j] = OFF;
          } else if(live >= bornThreshold && liveStep2==0) { 
            gen[i, j] = OFF;
          } else if (val == OFF && live >= bornThreshold) {
            gen[i, j] = ON;
          } else if (val == ON && live < surviveThreshold) {
            gen[i, j] = OFF;
          }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    private void PostProcess(int[,] map) {
      var dim1 = map.GetLength(0);
      var dim2 = map.GetLength(1);
      for (int i = 0; i < dim1; i++) {
        for (int j = 0; j < dim2; j++) {
          if (CanTrimCell(map, i, j)) {
            map[i, j] = OFF;
          }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="regions"></param>
    /// <returns></returns>
    private List<Region> RemoveSmallRegions(List<Region> regions){ 
      //Eliminate regions if they have a count of tiles less than or equal
      //to 2/5 of the total tiles number
      var minTiles = (int)(0.01*(Rows * Cols));
      var toRemove = regions
        .Where(r=>r.RegionFill==OFF && r.Tiles.Count <=minTiles).ToList();
      toRemove.ForEach(x=>regions.Remove(x));
      //mark the eliminated regions as ON on the map;
      toRemove.ForEach(r=>{ 
        r.Tiles.ForEach(t=>map[t.Row, t.Col]=ON);
      });
      return Region.ExtractRegions(map);
    }

    /// <summary>
    /// Ensure we do not have regions with a tile width
    /// </summary>
    /// <param name="map"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    private static bool IsTooNarrow(int[,] map, int i, int j){ 
      var dim1 = map.GetLength(0);
      var dim2 = map.GetLength(1);
      return 
        (i-1>=0 && j-1>=0 && i+1<dim1 && j+1<dim2 && 
          (
            (map[i-1,j]==OFF && map[i+1,j]==OFF) ||
            (map[i,j-1]==OFF && map[i,j+1]==OFF)
          )
        ) || 
        (i-1>=0 && j-1>=0 && i+1<dim1 && j+1<dim2 && 
          (
            (map[i-1,j-1]==OFF && map[i+1,j+1]==OFF) ||
            (map[i-1,j+1]==OFF && map[i+1,j-1]==OFF)
          )
        );
    }

    /// <summary>
    /// Trim the cells that have patterns like this:
    ///   X    #         #
    ///  ###   #X  ###  X#
    ///        #    X    #
    ///  The 
    ///  1 2 3
    ///  8 o 4
    ///  7 6 5 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    private static bool CanTrimCell(int[,] map, int i, int j) {
      var n = GetNeighbours(map, x => x == ON, i, j);
      var sum = n.Sum();
      if (n.Count() == 3 && (sum == 6 | sum == 12 || sum == 18 || sum == 16)) {
        return true;
      }
      if(n.Count()==0)
        return true;
      return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>s
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    private static IEnumerable<int> GetNeighbours(int[,] map, Func<int,bool> criteria, int i, int j, int step=1)
    {
      var dim1 = map.GetLength(0);
      var dim2 = map.GetLength(1);
      if(i==0 || j==0 || i==dim1-step || j == dim1-step){
        for (int ii = 0; ii < 8; ii++) {
          yield return ii+1;
        }
      } else {
        if (i - step >= 0 && j - step >= 0 && criteria(map[i - step, j - step])) yield return 1;
        if (i - step >= 0 && criteria(map[i - step, j])) yield return 2;
        if (i - step >= 0 && j + step < dim2 && criteria(map[i - step, j + step])) yield return 3;
        if (j + step < dim2 && criteria(map[i, j + step])) yield return 4;
        if (i + step < dim1 && j + step < dim2 && criteria(map[i + step, j + step])) yield return 5;
        if (i + step < dim1 && criteria(map[i + step, j])) yield return 6;
        if (i + step < dim1 && j - step >= 0 && criteria(map[i + step, j - step])) yield return 7;
        if (j - step >= 0 && criteria(map[i, j - step])) yield return 8;
      }
    }

    public void UpdateFrom(Region region) {
      region.Tiles.ForEach(x=>map[x.Row, x.Col]=region.RegionFill);
    }
  }
}
