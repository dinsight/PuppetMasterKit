using System;
using System.Collections.Generic;
using PuppetMasterKit.AI;

namespace PuppetMasterKit.Ios.Isometric.Tilemap
{
  public interface IRegionPainter
  {
    void Paint(Region region, TileMapLayer layer);
  }
}
