using PuppetMasterKit.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMasterKit.Terrain.Map.CellularAutomata
{
  class TrimAreasAutomaton : IAutomaton
  {
    public const int OFF = 0;
    private const int ON = 1;
    
    private int [,] map;
    private int rows;
    private int cols;
    private float smallRegionThresholdOn;
    private float smallRegionThresholdOff;

    public TrimAreasAutomaton(int [,] map, 
      float smallRegionThresholdOn,
      float smallRegionThresholdOff) {
      this.map = map;
      rows = map.GetLength(0);
      cols = map.GetLength(1);
      this.smallRegionThresholdOn = smallRegionThresholdOn;
      this.smallRegionThresholdOff = smallRegionThresholdOff;
    }

    public IAutomaton Run(int iterations) {
      for (int i = 0; i < iterations; i++) {
        Trim(map);
      }
      RemoveSmallRegions();
      return this;
    }

    public IAutomaton ThenRun(IAutomaton automaton, int iterations) {
      automaton.Run(iterations);
      return automaton;
    }

    private void Trim(int[,] map) {
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
    private static bool CanTrimCell(int[,] map, int i, int j) {
      var n = map.GetNeighbours( x => x == ON, i, j);
      var sum = n.Sum();
      if (n.Count() == 3 && (sum == 6 | sum == 12 || sum == 18 || sum == 16)) {
        return true;
      }
      if (n.Count() == 2 && (sum == 3 | sum == 5 || sum == 7 || sum == 9 
        || sum==11 || sum == 13 || sum == 15 ) ) {
        return true;
      }
      if(n.Count()==0)
        return true;
      return false;
    }

    private void RemoveSmallRegions(){ 
      var regions = Region.ExtractRegions(map);
      {
        //Eliminate regions if they have a count of tiles less than or equal
        //to 0.01 of the total tiles number
        var minTiles = (int)(smallRegionThresholdOff*(rows * cols));
        var toRemove = regions
          .Where(r=>r.RegionFill==OFF && r.Tiles.Count <=minTiles).ToList();
        toRemove.ForEach(x=>regions.Remove(x));
        //mark the eliminated regions as ON on the map;
        toRemove.ForEach(r=>{ 
          r.Tiles.ForEach(t=>map[t.Row, t.Col]=ON);
        });
      }
      {
        var minTiles = (int)(smallRegionThresholdOn*(rows * cols));
        var toRemove = regions
          .Where(r => r.RegionFill == ON && r.Tiles.Count <= minTiles).ToList();
        toRemove.ForEach(x => regions.Remove(x));
        //mark the eliminated regions as OFF on the map;
        toRemove.ForEach(r => {
          r.Tiles.ForEach(t => map[t.Row, t.Col] = OFF);
        });
      }
    }
  }
}
