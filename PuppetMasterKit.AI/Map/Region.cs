using PuppetMasterKit.AI.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pair = System.Tuple<int, int>;

namespace PuppetMasterKit.AI
{
  public class Region
  {
    public int RegionFill { get; }
    public IReadOnlyCollection<Tuple<int,int>> Tiles { 
      get { 
        return tiles.AsReadOnly();
        } 
    }

    private List<Tuple<int,int>> tiles = new List<Pair>();

    public Region(int regionFill) 
    {
      this.RegionFill = regionFill;
    }

    public void AddTile(int row, int col)
    { 
      tiles.Add(Tuple.Create(row,col));
    }
  }
}
