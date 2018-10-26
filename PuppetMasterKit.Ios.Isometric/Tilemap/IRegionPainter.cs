using System;
using System.Collections.Generic;
using PuppetMasterKit.AI;
using PuppetMasterKit.Terrain.Map;

namespace PuppetMasterKit.Ios.Isometric.Tilemap
{
  public interface IRegionPainter
  {
    void Paint(Region region, TileMapLayer layer);
  }
}
