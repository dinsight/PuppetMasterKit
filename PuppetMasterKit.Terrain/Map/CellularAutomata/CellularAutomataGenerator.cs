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
    private readonly int generations;
    private readonly Func<int, int, int> seed;
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
      Func<int,int,int> seed)
    {
      this.generations = generations;
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
        Generate(prev, current);
        var temp = prev;
        prev = current;
        current = temp;
      }
      map = prev;
      return Region.ExtractRegions(map);
    }

    /// <summary>
    /// 
    /// </summary>
    private void Initialize() {
      for (int i = 0; i < Rows; i++) {
        for (int j = 0; j < Cols; j++) {
          map[i, j] = seed(i, j);
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="genPrev"></param>
    /// <param name="gen"></param>
    private void Generate(int[,] genPrev, int[,] gen)
    {
      for (int i = 0; i < Rows; i++) {
        for (int j = 0; j < Cols; j++) {
          var val = genPrev[i, j];
          var live = GetNeighbours(genPrev, i, j).Count(x => x > 0);
          //B678/S345678
          gen[i, j] = genPrev[i, j];
          if (val == OFF && live >= 6) {
            gen[i, j] = ON;
          }
          if (val == ON && live < 3) {
            gen[i, j] = OFF;
          }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns></returns>
    private static IEnumerable<int> GetNeighbours(int[,] map, int i, int j)
    {
      var dim1 = map.GetLength(0);
      var dim2 = map.GetLength(1);
      if (i - 1 >= 0 && j - 1 >= 0) yield return map[i - 1, j - 1];
      if (i - 1 >= 0) yield return map[i - 1, j];
      if (i - 1 >= 0 && j + 1 < dim2) yield return map[i - 1, j + 1];
      if (j - 1 >= 0) yield return map[i, j - 1];
      if (j + 1 < dim2) yield return map[i, j + 1];
      if (i + 1 < dim1 && j - 1 >= 0) yield return map[i + 1, j - 1];
      if (i + 1 < dim1) yield return map[i + 1, j];
      if (i + 1 < dim1 && j + 1 < dim2) yield return map[i + 1, j + 1];
    }
  }
}
