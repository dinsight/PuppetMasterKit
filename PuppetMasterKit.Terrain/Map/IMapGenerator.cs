using System;
using System.Collections.Generic;
using PuppetMasterKit.Utility.Subscript;

namespace PuppetMasterKit.Terrain.Map
{
  public interface IMapGenerator : I2DSubscript<int?>
  {
    List<Region> Create(int rows, int cols);
    
    void UpdateFrom(Region region);
  }
}
