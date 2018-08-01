using System;
using System.Linq;
using System.Collections.Generic;
using CoreGraphics;
using SpriteKit;
using PuppetMasterKit.AI;
using Pair = System.Tuple;

namespace PuppetMasterKit.Tilemap
{
  public class TileMap : SKNode
  {
    private List<TileMapLayer> layers = new List<TileMapLayer>();
    private Dictionary<int, string> tileMapping;
    private SKTileSet tileSet;
    private int[,] map;
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
    private const string UPPER_RIGHT_EDGE = "Upper Right Edge";
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

    private bool IsValid(int i, int j) => i >= 0 && j >= 0 && i < map.GetLength(0) && j < map.GetLength(1);
    private bool Empty(int i, int j, int v) => !Filled(i,j,v);
    private bool Filled(int i, int j, int v) => IsValid(i, j) && map[i, j] == v;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Tilemap.TileMap"/> class.
    /// </summary>
    /// <param name="map">Map.</param>
    /// <param name="tileMapping">Tile mapping.</param>
    /// <param name="tileSet">Tile set.</param>
    /// <param name="tileHeight">Tile height.</param>
    /// <param name="tileWidth">Tile width.</param>
    public TileMap(int[,] map, 
                   Dictionary<int, string> tileMapping, 
                   SKTileSet tileSet, 
                   int tileHeight, 
                   int tileWidth)
    {
      this.rows = map.GetLength(0);
      this.cols = map.GetLength(1);
      this.tileHeight = tileHeight;
      this.tileWidth = tileWidth;
      this.tileMapping = tileMapping;
      this.tileSet = tileSet;
      this.map = map;
    }
    /// <summary>
    /// Build this instance.
    /// </summary>
    public void Build(params int[] order)
    {
      var regions = ExtractRegions(map);
      PaintRegions(regions, order);
    }

    /// <summary>
    /// Extracts the regions.
    /// </summary>
    /// <returns>The regions.</returns>
    /// <param name="geography">Geography.</param>
    private List<Region> ExtractRegions(int[,] geography)
    {
      var regions = new Dictionary<int, Region>();
      for (int row = 0; row < rows; row++) {
        for (int col = 0; col < cols; col++) {
          var val = geography[row, col];
          Region region = null;
          if(regions.ContainsKey(val)){
            region = regions[val];
          } else {
            region = new Region(val);
            regions.Add(val, region);
          }
          region.AddTile(row, col);
        }
      }
      return regions.Values.ToList();
    }

    /// <summary>
    /// Paints the regions.
    /// </summary>
    /// <param name="regions">Regions.</param>
    private void PaintRegions(List<Region> regions, int[] order)
    {
      var baseTileLayer = CreateLayer(-1);
      var topTileLayer = CreateLayer(1);

      //if a preferred build order is given, apply it to the region list
      if(order!=null){
        var orderedList = order.ToList();
        //Sort the regions based on the list provided
        regions = regions.OrderBy(reg => {
          var index = orderedList.IndexOf(reg.RegionFill);
          return index < 0 ? int.MaxValue : index;
        } ).ToList();
      }
      //Select the tiles for each region and apply the corresponding texture
      regions.ForEach(reg => {
        var tileGroup = tileMapping
          .Where(k => k.Key == reg.RegionFill)
          .Select(x => tileSet
                  .TileGroups
                  .FirstOrDefault(t => t.Name == x.Value))
          .FirstOrDefault();
        
        foreach (var tile in reg.Tiles) {
          if(tileGroup!=null){            
            //set the appropriate texture for the adjacent tiles
            SetAdjacentTiles(baseTileLayer, 
                             tileGroup, 
                             tile.Item1, 
                             tile.Item2);
            //set the tile texture
            baseTileLayer.SetTile(GetTexture(tileGroup, CENTER),
                                  tile.Item1,
                                  tile.Item2);
          }
        }
      });
    }

    /// <summary>
    /// Sets the adjacent tiles.
    /// </summary>
    /// <param name="layer">Layer.</param>
    /// <param name="tileGroup">Tile group.</param>
    /// <param name="i">The index.</param>
    /// <param name="j">J.</param>
    private void SetAdjacentTiles(TileMapLayer layer, 
                                  SKTileGroup tileGroup, 
                                  int i, int j)
    {
      var v = map[i, j];
      int? z = null;
      //NE
      if (Filled(i - 1, j - 1, v) && Empty(i - 1, j, v) && Empty(i - 1, j + 1, v)) {
        layer.SetTile(GetTexture(tileGroup, LOWER_LEFT_CORNER), i - 1, j, z);
      }
      if (Empty(i - 1, j - 1, v) && Empty(i - 1, j, v) && Filled(i - 1, j + 1, v)) {
        layer.SetTile(GetTexture(tileGroup, LOWER_RIGHT_CORNER), i - 1, j, z);
      }
      if (Empty(i - 1, j - 1, v) && Empty(i - 1, j, v) && Empty(i - 1, j + 1, v)) {
        layer.SetTile(GetTexture(tileGroup, UP_EDGE), i - 1, j, z);
      }
      //SE
      if (Filled(i - 1, j + 1, v) && Empty(i, j + 1, v) && Empty(i + 1, j + 1, v)) {
        layer.SetTile(GetTexture(tileGroup, UPPER_LEFT_CORNER), i, j + 1, z);
      }
      if (Empty(i - 1, j + 1, v) && Empty(i, j + 1, v) && Filled(i + 1, j + 1, v)) {
        layer.SetTile(GetTexture(tileGroup, LOWER_LEFT_CORNER), i, j + 1, z);
      }
      if (Empty(i - 1, j + 1, v) && Empty(i, j + 1, v) && Empty(i + 1, j + 1, v)) {
        layer.SetTile(GetTexture(tileGroup, RIGHT_EDGE), i, j + 1, z);
      }
      //SW
      if (Filled(i + 1, j + 1, v) && Empty(i + 1, j, v) && Empty(i + 1, j - 1, v)) {
        layer.SetTile(GetTexture(tileGroup, UPPER_RIGHT_CORNER), i + 1, j, z);
      }
      if (Empty(i + 1, j + 1, v) && Empty(i + 1, j, v) && Filled(i + 1, j - 1, v)) {
        layer.SetTile(GetTexture(tileGroup, UPPER_LEFT_CORNER), i + 1, j, z);
      }
      if (Empty(i + 1, j + 1, v) && Empty(i + 1, j, v) && Empty(i + 1, j - 1, v)) {
        layer.SetTile(GetTexture(tileGroup, DOWN_EDGE), i + 1, j, z);
      }
      //NW
      if (Filled(i + 1, j - 1, v) && Empty(i, j - 1, v) && Empty(i - 1, j - 1, v)) {
        layer.SetTile(GetTexture(tileGroup, LOWER_RIGHT_CORNER), i, j - 1);
      }
      if (Empty(i + 1, j - 1, v) && Empty(i, j - 1, v) && Filled(i - 1, j - 1, v)) {
        layer.SetTile(GetTexture(tileGroup, UPPER_RIGHT_CORNER), i, j - 1);
      }
      if (Empty(i + 1, j - 1, v) && Empty(i, j - 1, v) && Empty(i - 1, j - 1, v)) {
        layer.SetTile(GetTexture(tileGroup, LEFT_EDGE), i, j - 1);
      }
      //N corner
      if (Empty(i, j - 1, v) && Empty(i - 1, j - 1, v) && Empty(i - 1, j, v)) {
        layer.SetTile(GetTexture(tileGroup, UPPER_LEFT_EDGE), i - 1, j - 1);
      }
      ////E corner
      if (Empty(i - 1, j, v) && Empty(i - 1, j + 1, v) && Empty(i, j + 1, v)) {
        layer.SetTile(GetTexture(tileGroup, UPPER_RIGHT_EDGE), i - 1, j + 1);
      }
      //S corner
      if (Empty(i, j + 1, v) && Empty(i + 1, j + 1, v) && Empty(i + 1, j, v)) {
        layer.SetTile(GetTexture(tileGroup, LOWER_RIGHT_EDGE), i + 1, j + 1, z);
      }
      ////W corner
      if (Empty(i + 1, j, v) && Empty(i + 1, j - 1, v) && Empty(i, j - 1, v)) {
        layer.SetTile(GetTexture(tileGroup, LOWER_LEFT_EDGE), i + 1, j - 1);
      }
    }

    /// <summary>
    /// Gets the texture.
    /// </summary>
    /// <returns>The texture.</returns>
    /// <param name="group">Group.</param>
    private SKTexture GetTexture(SKTileGroup group, string ruleName)
    {
      if (group.Rules.Count() == 1) {
        return group.Rules
                    .First()
                    .TileDefinitions
                    .First()
                    .Textures
                    .First();
      }
      return group.Rules
                  .Where(r => r.Name == ruleName)
                  .Select(x=>x.TileDefinitions.FirstOrDefault())
                  .Where(t=>t != null)
                  .Select(d=>d.Textures.First())
                  .FirstOrDefault();
    }

    /// <summary>
    /// Creates the layer.
    /// </summary>
    /// <returns>The layer.</returns>
    private TileMapLayer CreateLayer(int zPos)
    {
      var layer = new TileMapLayer(this);
      layer.ZPosition = zPos;
      layers.Add(layer);
      layer.Position = new CGPoint(0, 0);
      this.AddChild(layer);
      return layer;
    }
  }
}
