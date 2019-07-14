using System;
using System.Collections.Generic;

namespace PuppetMasterKit.Terrain.Map
{
  public interface IMapGenerator
  {
    List<Region> Create(int rows, int cols);
  }
}
