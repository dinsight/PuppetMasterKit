using System;
using System.Linq;
using System.Collections.Generic;
using CoreGraphics;
using SpriteKit;

namespace PuppetMasterKit.Tilemap
{
  public class TileMap : SKNode
  {
    private List<TileMapLayer> layers = new List<TileMapLayer>();
    private int rows;
    private int cols;
    private int tileHeight;
    private int tileWidth;

    public int Rows { get => rows; }
    public int Cols { get => cols; }
    public int TileHeight { get => tileHeight; }
    public int TileWidth { get => tileWidth; }

    private const string CENTER = "Center";
    private const string UP_EDGE = "Up Edge";
    private const string UP_RIGHT_EDGE = "Upper Right Edge";
    private const string RIGHT_EDGE = "Right Edge";
    private const string LOWER_RIGHT_EDGE = "Lower Right Edge";
    private const string DOWN_EDGE = "Down Edge";
    private const string LOWER_LEFT_EDGE = "Lower Left Edge";
    private const string LEFT_EDGE = "Left Edge";
    private const string UPPER_LEFT_EDGE = "Upper Left Edge";
    private const string UPPER_RIGHT_CORNER = "Upper Right Corner";
    private const string LOWER_RIGHT_CORNER = "Lower Right Corner";
    private const string LOWER_LEFT_CORNER = "Lower Left Corner";
    private const string UPPER_LEFT_CORNER = "Upper Left Corner";

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Tilemap.TileMap"/> class.
    /// </summary>
    /// <param name="rows">Rows.</param>
    /// <param name="cols">Cols.</param>
    /// <param name="tileHeight">Tile height.</param>
    /// <param name="tileWidth">Tile width.</param>
    public TileMap(int rows, int cols, int tileHeight, int tileWidth)
    {
      this.rows = rows;
      this.cols = cols;
      this.tileHeight = tileHeight;
      this.tileWidth = tileWidth;
    }

    /// <summary>
    /// Creates from.
    /// </summary>
    /// <param name="map">Map.</param>
    /// <param name="tileMapping">Tile mapping.</param>
    /// <param name="tileSet">Tile set.</param>
    public void CreateFrom(int[,] map, Dictionary<int, string> tileMapping, SKTileSet tileSet)
    {
      var baseTileLayer = CreateLayer();
      var topTileLayer = CreateLayer();

      baseTileLayer.ZPosition = -1;
      topTileLayer.ZPosition = 1;

      for (int row = 0; row < map.GetLength(0); row++) {
        for (int col = 0; col < map.GetLength(1); col++) {
          var val = map[row, col];
          var texture = GetTexture(val, tileMapping, tileSet);
          //var split = TileHelper.SplitTile(texture, tileWidth, tileHeight);
          baseTileLayer.SetTile(texture, row, col);
        }
      }
    }

    private string GetRuleForPos(int[,] map, int row, int col)
    {
      return CENTER;
    }

    /// <summary>
    /// Gets the texture.
    /// </summary>
    /// <returns>The texture.</returns>
    /// <param name="val">Value.</param>
    /// <param name="tileMapping">Tile mapping.</param>
    /// <param name="tileSet">Tile set.</param>
    private SKTexture GetTexture(int val,
                                 Dictionary<int, string> tileMapping,
                                 SKTileSet tileSet)
    {
      if (tileMapping.ContainsKey(val)) {
        var tileGroupName = tileMapping[val];
        var tileGroup = tileSet.TileGroups.FirstOrDefault(x => x.Name == tileGroupName);
        if (tileGroup != null) {
          return GetTexture(tileGroup, CENTER);
        }
      }
      return null;
    }

    /// <summary>
    /// Gets the texture.
    /// </summary>
    /// <returns>The texture.</returns>
    /// <param name="group">Group.</param>
    private SKTexture GetTexture(SKTileGroup group, string ruleName)
    {
      if (group.Rules.Count() == 1) {
        return group.Rules.First().TileDefinitions.First().Textures.First();
      }
      var rule = group.Rules.FirstOrDefault(r => r.Name == ruleName);
      if (rule == null)
        return null;

      var tileDef = rule.TileDefinitions.FirstOrDefault();
      if (tileDef == null)
        return null;
      return tileDef.Textures.FirstOrDefault();
    }

    /// <summary>
    /// Creates the layer.
    /// </summary>
    /// <returns>The layer.</returns>
    private TileMapLayer CreateLayer()
    {
      var layer = new TileMapLayer(this);
      layers.Add(layer);
      layer.Position = new CGPoint(0, 0);
      this.AddChild(layer);
      return layer;
    }
  }
}
