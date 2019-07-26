using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMasterKit.Terrain.Map.CellularAutomata
{
  class NarrowAreasAutomaton : IAutomaton
  {
    public const int OFF = 0;
    private const int ON = 1;

    private int bornThreshold;
    private int surviveThreshold;
    private int [,] map;
    private int rows;
    private int cols;

    public NarrowAreasAutomaton(int [,] map, int bornThreshold, int surviveThreshold) {
      this.bornThreshold = bornThreshold;
      this.surviveThreshold = surviveThreshold;
      this.map = map;
      rows = map.GetLength(0);
      cols = map.GetLength(1);
    }

    public IAutomaton Run(int iterations) {
      var prev = new int [rows, cols];
      var current = new int [rows, cols];
      map.CopyTo(prev);
      for (int i = 0; i < iterations; i++) {
        Generate(prev, current);
        var temp = prev;
        prev = current;
        current = temp;
      }
      prev.CopyTo(map);
      return this;
    }

    public IAutomaton ThenRun(IAutomaton automaton, int iterations) {
      automaton.Run(iterations);
      return automaton;
    }
  
    private void Generate(int[,] genPrev, int[,] gen) {
      for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++) {
          var live1 = genPrev.GetNeighbours(x => x > 0, i, j).Count();
          var live2 = genPrev.GetNeighbours(x => x > 0, i, j, 2).Count();
          gen[i, j] = genPrev[i, j];
          if (IsTooNarrow(genPrev, i, j)) {
            gen[i, j] = OFF;
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
  }
}
