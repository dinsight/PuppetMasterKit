using System;
using System.Collections.Generic;
using System.Linq;
using PuppetMasterKit.Utility.Extensions;
using PuppetMasterKit.Terrain.Map;
using PuppetMasterKit.Utility;
using SpriteKit;

namespace PuppetMasterKit.Ios.Tiles.Tilemap.Painters
{
  public class LayeredRegionPainter : IRegionPainter
  {
    private readonly int noiseAmplitude;
    private readonly SKTileSet tileSet;
    private readonly List<string> layers;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Ios.Isometric.Tilemap.LayeredRegionPainter"/> class.
    /// </summary>
    /// <param name="noiseAmplitude">Noise amplitude.</param>
    /// <param name="layers">Layers.</param>
    /// <param name="tileSet">Tile set.</param>
    public LayeredRegionPainter(int noiseAmplitude,
          List<string> layers,
          SKTileSet tileSet)
    {
      this.noiseAmplitude = noiseAmplitude;
      this.tileSet = tileSet;
      this.layers = layers;
    }

    /// <summary>
    /// Paint the specified region and layer.
    /// </summary>
    /// <param name="region">Region.</param>
    /// <param name="layer">Layer.</param>
    public void Paint(Region region, TileMapLayer layer)
    {
      if (region.Type != Region.RegionType.REGION) {
        throw new ArgumentException("LayeredRegionPainter: Unsupported region");
      }
      if (layers.Count < 3) {
        throw new ArgumentException("LayeredRegionPainter: At least three layers need to be defined");
      }


      var layersCount = layers.Count;
      var partitions = PartitionRegion(region, layer.GetMap());
      var indexRange = ((float)partitions.Count / layersCount);
      ScramblePartitions(partitions);

      partitions.ForEach((p, index) => {
        var ind = (int)(index / indexRange);
        if (index <= 1) {
          p.ForEach(x => region[x.Row, x.Col] = index);
        } else if (ind < 2) {
          p.ForEach(x => region[x.Row, x.Col] = 2);
        } else {
          p.ForEach(x => region[x.Row, x.Col] = ind);
        }
      });

      var regions = Region.ExtractRegions(region);
      var tileMapping = new Dictionary<int, string>();
      var tiledRegionPainter = new TiledRegionPainter(tileMapping, tileSet);
      regions.ForEach(r => tileMapping.TryAdd(r.RegionFill, layers[r.RegionFill]));
      regions.OrderBy(r => r.RegionFill).ToList()
             .ForEach(x => tiledRegionPainter.Paint(x, layer));
    }

    /// <summary>
    /// Partitions the region.
    /// </summary>
    /// <param name="region">Region.</param>
    private List<HashSet<GridCoord>> PartitionRegion(Region region, TileMap map)
    {
      var contour = region.TraceContour(false)
          .Where(x => x.Row != 0 && x.Row != map.Rows - 1
                  && x.Col != 0 && x.Col != map.Cols - 1).ToList();

      var s = new HashSet<GridCoord>(contour);
      var v = new List<GridCoord>(region.Tiles.Except(contour));
      var a = new List<HashSet<GridCoord>> { new HashSet<GridCoord>(contour) };

      while (v.Count > 0) {
        var r = v.Where(x => IsInVicinity(x, s)).ToList();
        if (!r.Any())
          break;
        s.UnionWith(r);
        v = v.Except(r).ToList();
        a.Add(new HashSet<GridCoord>(r));
      }
      return a;
    }

    /// <summary>
    /// Scrambles the partitions.
    /// </summary>
    /// <param name="a">The alpha component.</param>
    private void ScramblePartitions(List<HashSet<GridCoord>> a)
    {
      var random = new Random(1);
      a.ForEach((x, index) => {
        if (index > 1) {
          var tmp = x
            .Where((y, j) => random.Next(0, 10) % 2 == 0).ToList();
          x.RemoveWhere(tmp.Contains);
          a[index - 1].UnionWith(tmp);
        }
      });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns><c>true</c>, if neighbor was ised, <c>false</c> otherwise.</returns>
    /// <param name="elem">Element.</param>
    /// <param name="set">Set.</param>
    private bool IsInVicinity(GridCoord elem, HashSet<GridCoord> set)
    {
      IEnumerable<GridCoord> GetNeighbours(GridCoord e)
      {
        //yield return new GridCoord(e.Row + 1, e.Col - 1);
        yield return new GridCoord(e.Row, e.Col - 1);
        //yield return new GridCoord(e.Row - 1, e.Col - 1);
        yield return new GridCoord(e.Row + 1, e.Col);
        yield return new GridCoord(e.Row - 1, e.Col);
        //yield return new GridCoord(e.Row + 1, e.Col + 1);
        yield return new GridCoord(e.Row, e.Col + 1);
        //yield return new GridCoord(e.Row - 1, e.Col + 1);
      }
      var neighbours = GetNeighbours(elem);
      return neighbours.Any(x => set.Contains(x));
    }
  }
}