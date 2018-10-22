using System;
using System.Linq;
using System.Collections.Generic;
using CoreGraphics;
using SpriteKit;
using PuppetMasterKit.AI;
using PuppetMasterKit.Utility;
using Pair = System.Tuple;
using CoreImage;

namespace PuppetMasterKit.Ios.Isometric.Tilemap
{
  public class TileMap : SKNode
  {
    #region Private members
    private readonly Dictionary<int, IRegionPainter> regionPainter = new Dictionary<int, IRegionPainter>();

    private readonly Random random = new Random(Guid.NewGuid().GetHashCode());

    private readonly List<TileMapLayer> layers = new List<TileMapLayer>();

    private readonly IRegionPainter defaultPainter;
    #endregion

    #region Public properties
    public int Rows { get; }

    public int Cols { get; }

    public int TileSize { get; }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Ios.Isometric.Tilemap.TileMap"/> class.
    /// </summary>
    /// <param name="map">Map.</param>
    /// <param name="defaultPainter">Default painter.</param>
    /// <param name="tileSize">Tile size.</param>
    public TileMap(IRegionPainter defaultPainter, int rows, int cols, int tileSize)
    {
      this.Rows = rows;
      this.Cols = cols;
      this.TileSize = tileSize;
      this.defaultPainter = defaultPainter;
    }

    /// <summary>
    /// Build this instance.
    /// </summary>
    public void Build(IReadOnlyCollection<Region> regions, params int[] order)
    {
      var baseTileLayer = CreateLayer(-1);
      var topTileLayer = CreateLayer(1);
      var thirdLayer = CreateLayer(2);

      //if a preferred build order is given, apply it to the region list
      if (order != null) {
        var orderedList = order.ToList();
        //Sort the regions based on the list provided
        regions = regions.OrderBy(reg => {
          var index = orderedList.IndexOf(reg.RegionFill);
          return index < 0 ? int.MaxValue : index;
        }).ToList();
      }
      //var painter = new TiledRegionPainter(tileMapping, tileSet, map);
      //Select the tiles for each region and apply the corresponding texture
      regions.ForEach(reg => {
        IRegionPainter painter = defaultPainter;
        if (regionPainter.ContainsKey(reg.RegionFill)) {
          painter = regionPainter[reg.RegionFill];
        }
        painter.Paint(reg, baseTileLayer);
      });
    }

    /// <summary>
    /// Adds special painters for specific regions.
    /// </summary>
    /// <param name="regionFill">Region fill.</param>
    /// <param name="painter">Painter.</param>
    public void AddPainter(int regionFill, IRegionPainter painter)
    {
      if (regionPainter.ContainsKey(regionFill)){
        regionPainter[regionFill] = painter;
      } else {
        regionPainter.Add(regionFill, painter);
      }
    }

    /// <summary>
    /// Gets the layer.
    /// </summary>
    /// <returns>The layer.</returns>
    /// <param name="index">Index.</param>
    public TileMapLayer GetLayer(int index)
    {
      return layers[index];
    }

    /// <summary>
    /// Flattens the layer.
    /// </summary>
    /// <returns>The layer.</returns>
    /// <param name="index">Index.</param>
    public SKSpriteNode FlattenLayer(int index, Action<CGImage> debug = null)
    {
      if (index >= 0 && index < layers.Count) {
        return layers[index].FlattenLayer(debug);
      }
      return null;
    }

    #region Private Methods
    /// <summary>
    /// Creates the layer.
    /// </summary>
    /// <returns>The layer.</returns>
    /// <param name="zPos">Z position.</param>
    private TileMapLayer CreateLayer(int zPos)
    {
      var layer = new TileMapLayer(this, layers.Count) {
        ZPosition = zPos,
        Position = new CGPoint(0, 0),
      };
      layers.Add(layer);
      this.AddChild(layer);
      return layer;
    }
    #endregion
  }
}
