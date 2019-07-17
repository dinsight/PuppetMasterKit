using System;
using PuppetMasterKit.Terrain.Map;

namespace PuppetMasterKit.Ios.Tiles.Tilemap.Painters
{
  public interface IRegionPainter
  {
    void Paint(Region region, TileMapLayer layer);
  }
}
