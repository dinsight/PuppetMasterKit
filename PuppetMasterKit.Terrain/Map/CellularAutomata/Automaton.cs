using PuppetMasterKit.Terrain.Map.CellularAutomata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuppetMasterKit.Terrain.Map.CellularAutomata;

namespace PuppetMasterKit.Terrain.Map.CellularAutomata
{
  class Automaton : IAutomaton
  {
    public const int OFF = 0;
    private const int ON = 1;

    private int bornThreshold;
    private int surviveThreshold;
    private int [,] map;
    private int rows;
    private int cols;

    public Automaton(int [,] map, int bornThreshold, int surviveThreshold) {
      this.bornThreshold = bornThreshold;
      this.surviveThreshold = surviveThreshold;
      this.map = map;
      rows = map.GetLength(0);
      cols = map.GetLength(1);
    }

    public IAutomaton Run(int iterations) {
      var prev = map;
      var current = new int [rows, cols];
      
      for (int i = 0; i < iterations; i++) {
        Generate(prev, current);
        var temp = prev;
        prev = current;
        current = temp;
      }
      return this;
    }

    public IAutomaton ThenRun(IAutomaton automaton, int iterations) {
      automaton.Run(iterations);
      return automaton;
    }

    private void Generate(int[,] genPrev, int[,] gen) {
      for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++) {
          var val = genPrev[i, j];
          var live = genPrev.GetNeighbours( x => x > 0, i, j).Count();
          var liveStep2 = genPrev.GetNeighbours(x => x > 0, i, j, 2).Count();

          gen[i, j] = genPrev[i, j];
          if (val == OFF && live >= bornThreshold) {
            gen[i, j] = ON;
          } else if (val == ON && live < surviveThreshold) {
            gen[i, j] = OFF;
          }
        }
    }
  }
}
