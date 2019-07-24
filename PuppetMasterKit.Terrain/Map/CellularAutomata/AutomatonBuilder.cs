using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMasterKit.Terrain.Map.CellularAutomata
{
  public class AutomatonBuilder
  {
    private IAutomaton automaton;

    private int [,] map;

    Func<int,int,int, int, int> seed;

    private AutomatonBuilder(int [,] map){
      this.map = map;
    }

    public static AutomatonBuilder For(int [,] map) { 
      AutomatonBuilder builder = new AutomatonBuilder(map);
      //automaton = new Automaton(map, bornThreshold, surviveThreshold);
      return builder;
    }

    //int bornThreshold, int surviveThreshold

    public AutomatonBuilder With(Func<int,int,int, int, int> seed){ 
      var rows = map.GetLength(0);
      var cols = map.GetLength(1);
      for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
          map[i, j] = seed(i, j, rows, cols);
        }
      }
      return this;
    }

    public IAutomaton Build(){ 
      return automaton;
    }
  }
}
