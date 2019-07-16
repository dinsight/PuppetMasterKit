using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.Utility.Extensions;
using System;

namespace PuppetMasterKit.Terrain.Map.CellularAutomata
{
  public class CellularAutomataGenerator : IMapGenerator
  {
    private const int OFF = 0;
    private const int ON = 1;
    private const int WATER = 3;
    private readonly int generations;
    private int bornThreshold;
    private int surviveThreshold;
    private readonly Func<int, int, int , int, int> seed;
    private int[,] map;

    public int Rows { get; private set; }
    public int Cols { get; private set; }
    public int this[int row, int col] => map[row,col];

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
      return Postprocess(Region.ExtractRegions(map));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="regions"></param>
    /// <returns></returns>
    private List<Region> Postprocess(List<Region> regions){ 
      //Eliminate regions if they have a count of tiles less than or equal
      //to 2/5 of the total tiles number
      var minTiles = (int)(0.01*(Rows * Cols));
      var lakes = (int)(0.03*(Rows * Cols));
      var toRemove = regions
        .Where(r=>r.RegionFill==OFF && r.Tiles.Count <=minTiles).ToList();
      toRemove.ForEach(x=>regions.Remove(x));
      //mark the eliminated regions as ON on the map;
      toRemove.ForEach(r=>{ 
        r.Tiles.ForEach(t=>map[t.Row, t.Col]=ON);
      });
      //mark the remaining isolated regions as lakes 
      regions
        .Where(r=>r.RegionFill==OFF && r.Tiles.Count <=lakes).ToList()
        .ForEach(r=>{ 
          r.Tiles.ForEach(t=>map[t.Row, t.Col]=WATER);
        });

      return regions;
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
    /// <param name="currentGeneration"></param>
    /// <param name="genPrev"></param>
    /// <param name="gen"></param>
    private void Generate(int currentGeneration, int[,] genPrev, int[,] gen)
    {
      for (int i = 0; i < Rows; i++) {
        for (int j = 0; j < Cols; j++) {
          var val = genPrev[i, j];
          var live = GetNeighbours(genPrev, i, j).Count(x => x > 0);
          var liveStep2 = GetNeighbours(genPrev, i, j, 2).Count(x => x > 0);
          gen[i, j] = genPrev[i, j];

          if(IsTooNarrow(genPrev, i, j) && currentGeneration == generations-1){ 
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
        (i-1>=0 && i+1<dim1 && map[i-1,j]==OFF && map[i+1,j]==OFF) || 
        (j-1>=0 && j+1<dim2 && map[i,j-1]==OFF && map[i,j+1]==OFF);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>s
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    private static IEnumerable<int> GetNeighbours(int[,] map, int i, int j, int step=1)
    {
      var dim1 = map.GetLength(0);
      var dim2 = map.GetLength(1);
      if(i==0 || j==0 || i==dim1-step || j == dim1-step){
        for (int ii = 0; ii < 8; ii++) {
          yield return 1;
        }
      } else {
        if (i - step >= 0 && j - step >= 0) yield return map[i - step, j - step];
        if (i - step >= 0) yield return map[i - step, j];
        if (i - step >= 0 && j + step < dim2) yield return map[i - step, j + step];
        if (j - step >= 0) yield return map[i, j - step];
        if (j + step < dim2) yield return map[i, j + step];
        if (i + step < dim1 && j - step >= 0) yield return map[i + step, j - step];
        if (i + step < dim1) yield return map[i + step, j];
        if (i + step < dim1 && j + step < dim2) yield return map[i + step, j + step];
      }
    }
  }
}
