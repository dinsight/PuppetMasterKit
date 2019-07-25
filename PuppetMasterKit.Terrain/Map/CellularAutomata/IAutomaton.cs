using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMasterKit.Terrain.Map.CellularAutomata
{
  public interface IAutomaton {
    IAutomaton Run(int iterations);

    IAutomaton ThenRun(IAutomaton automaton, int iterations);
  }
}
