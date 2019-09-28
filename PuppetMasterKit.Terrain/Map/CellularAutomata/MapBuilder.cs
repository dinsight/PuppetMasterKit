using PuppetMasterKit.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMasterKit.Terrain.Map.CellularAutomata
{
  public class MapBuilder
  {
    Func<int,int,int, int, int> seed;
    private int birthThreshold = 5;
    private int survivalThreshold = 4;
    private float onThreshold = 0.01f;
    private float offThreshold = 0.009f;

    private MapBuilder(){
    }

    public static MapBuilder Create() { 
      MapBuilder builder = new MapBuilder();
      return builder;
    }

    public MapBuilder With(Func<int,int,int, int, int> seed){ 
      this.seed = seed;
      return this;
    }
    public MapBuilder WithBirthThreshold(int birthThreshold){ 
      this.birthThreshold = birthThreshold;
      return this;
    }

    public MapBuilder WithSurvivalThreshold(int survivalThreshold){ 
      this.survivalThreshold = survivalThreshold;
      return this;
    }

    public MapBuilder WithOnRegionThreshold(float onThreshold){ 
      this.onThreshold = onThreshold;
      return this;
    }

    public MapBuilder WithOffRegionThreshold(float offThreshold){ 
      this.offThreshold = offThreshold;
      return this;
    }

    public IMapGenerator Build(){ 
      var gen = new CellularAutomatonGenerator();
      gen.seed = seed;
      gen.BirthThreshold = birthThreshold;
      gen.SurvivalThreshold = survivalThreshold;
      gen.OnThreshold = onThreshold;
      gen.OffThreshold = offThreshold;
      return gen;
    }

    private class CellularAutomatonGenerator : IMapGenerator
    {
      private int[,] map;
      private int rows;
      private int cols;
      
      public Func<int,int,int, int, int> seed;
      public int? this[int row, int col] => map[row,col];

      public int Rows => rows;

      public int Cols => cols;
      public int SurvivalThreshold { get ; set; }
      public int BirthThreshold { get ; set; }
      public float OnThreshold{ get ; set; }
      public float OffThreshold { get ; set; }

      public CellularAutomatonGenerator() {
        this.map = new int[0, 0];
      }

      public List<Region> Create(int rows, int cols) {
        this.rows = rows;
        this.cols = cols;
        this.map = new int[rows, cols];
        if(seed!=null){
          for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
              map[i, j] = seed(i, j, rows, cols);
            }
          }
        }

        var automaton = new Automaton(map, BirthThreshold, SurvivalThreshold);
        var faAutomaton = new FillAreasAutomaton(map, BirthThreshold, SurvivalThreshold);
        var naAutomaton = new NarrowAreasAutomaton(map, BirthThreshold, SurvivalThreshold);
        var trimAutomaton = new TrimAreasAutomaton(map, OnThreshold, OffThreshold);
        var automaton55 = new Automaton(map, 5, 5);
        automaton.Run(20)
          .ThenRun(faAutomaton, 1)
          .ThenRun(naAutomaton, 1)
          .ThenRun(automaton, 5)
          .ThenRun(automaton55,1)
          .ThenRun(automaton, 2)
          .ThenRun(trimAutomaton,3)
          ;

        return Region.ExtractRegions(map);
      }

      public void UpdateFrom(Region region) {
        region.Tiles.ForEach(x=>map[x.Row, x.Col]=region.RegionFill);
      }
      
    }
  }
}
