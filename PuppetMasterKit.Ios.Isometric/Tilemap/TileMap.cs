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
    Random random = new Random(Guid.NewGuid().GetHashCode());
    private List<TileMapLayer> layers = new List<TileMapLayer>();
    private List<Region> regions = new List<Region>();
    private Dictionary<int, string> tileMapping;
    private SKTileSet tileSet;
    private int[,] map;
    private int rows;
    private int cols;
    private int tileSize;

    public int Rows { get => rows; }
    public int Cols { get => cols; }
    public int TileSize { get => tileSize; }

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
    /// <param name="tileSize">Tile size.</param>
    public TileMap(int[,] map, 
                   Dictionary<int, string> tileMapping, 
                   SKTileSet tileSet, 
                   int tileSize)
    {
      this.rows = map.GetLength(0);
      this.cols = map.GetLength(1);
      this.tileSize = tileSize;
      this.tileMapping = tileMapping;
      this.tileSet = tileSet;
      this.map = map;
    }
    /// <summary>
    /// Build this instance.
    /// </summary>
    public void Build(params int[] order)
    {
      regions = Region.ExtractRegions(map);
      PaintRegions(order);
    }

    /// <summary>
    /// Paints the regions.
    /// </summary>
    /// <param name="order">Order.</param>
    private void PaintRegions(int[] order)
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

        var corners = GetCorners(tileGroup);
        foreach (var tile in reg.Tiles) {
          if(tileGroup!=null){            
            //set the appropriate texture for the adjacent tiles
            SetAdjacentTiles(baseTileLayer, 
                             corners, 
                             tile.Item1, 
                             tile.Item2);
            //set the tile texture
            baseTileLayer.SetTile(tileGroup.GetTexture(CENTER),
                                  tile.Item1,
                                  tile.Item2);
          }
        }
      });
    }

    /// <summary>
    /// Gets the corners.
    /// </summary>
    /// <returns>The corners.</returns>
    /// <param name="tileGroup">Tile group.</param>
    private Dictionary<string,SKTexture> GetCorners(SKTileGroup tileGroup){
      var dictionary = new Dictionary<string, SKTexture>();

      var maintTile = tileGroup.GetTexture(CENTER);
      dictionary.Add(LOWER_LEFT_CORNER,  maintTile.BlendWithAlpha(tileGroup.GetTexture(LOWER_LEFT_CORNER)));
      dictionary.Add(LOWER_RIGHT_CORNER, maintTile.BlendWithAlpha(tileGroup.GetTexture(LOWER_RIGHT_CORNER)));
      dictionary.Add(UP_EDGE, maintTile.BlendWithAlpha(tileGroup.GetTexture(UP_EDGE)));
      dictionary.Add(UPPER_LEFT_CORNER, maintTile.BlendWithAlpha(tileGroup.GetTexture(UPPER_LEFT_CORNER)));
      dictionary.Add(RIGHT_EDGE, maintTile.BlendWithAlpha(tileGroup.GetTexture(RIGHT_EDGE)));
      dictionary.Add(UPPER_RIGHT_CORNER, maintTile.BlendWithAlpha(tileGroup.GetTexture(UPPER_RIGHT_CORNER)));
      dictionary.Add(DOWN_EDGE, maintTile.BlendWithAlpha(tileGroup.GetTexture(DOWN_EDGE)));
      dictionary.Add(LEFT_EDGE, maintTile.BlendWithAlpha(tileGroup.GetTexture(LEFT_EDGE)));
      dictionary.Add(UPPER_LEFT_EDGE, maintTile.BlendWithAlpha(tileGroup.GetTexture(UPPER_LEFT_EDGE)));
      dictionary.Add(UPPER_RIGHT_EDGE, maintTile.BlendWithAlpha(tileGroup.GetTexture(UPPER_RIGHT_EDGE)));
      dictionary.Add(LOWER_RIGHT_EDGE, maintTile.BlendWithAlpha(tileGroup.GetTexture(LOWER_RIGHT_EDGE)));
      dictionary.Add(LOWER_LEFT_EDGE, maintTile.BlendWithAlpha(tileGroup.GetTexture(LOWER_LEFT_EDGE)));

      return dictionary;
    }

    /// <summary>
    /// Sets the adjacent tiles.
    /// </summary>
    /// <param name="layer">Layer.</param>
    /// <param name="corners">Tile group.</param>
    /// <param name="i">The index.</param>
    /// <param name="j">J.</param>
    private void SetAdjacentTiles(TileMapLayer layer, 
                                  Dictionary<string, SKTexture> corners,
                                  int i, int j)
    {
      var v = map[i, j];
      int? z = null;

      //NE
      if (Filled(i - 1, j - 1, v) && Empty(i - 1, j, v) && Empty(i - 1, j + 1, v)) {
        layer.SetTile(corners[LOWER_LEFT_CORNER], i - 1, j, z);
      }
      if (Empty(i - 1, j - 1, v) && Empty(i - 1, j, v) && Filled(i - 1, j + 1, v)) {
        layer.SetTile(corners[LOWER_RIGHT_CORNER], i - 1, j, z);
      }
      if (Empty(i - 1, j - 1, v) && Empty(i - 1, j, v) && Empty(i - 1, j + 1, v)) {
        layer.SetTile(corners[UP_EDGE], i - 1, j, z);
      }
      //SE
      if (Filled(i - 1, j + 1, v) && Empty(i, j + 1, v) && Empty(i + 1, j + 1, v)) {
        layer.SetTile(corners[UPPER_LEFT_CORNER], i, j + 1, z);
      }
      if (Empty(i - 1, j + 1, v) && Empty(i, j + 1, v) && Filled(i + 1, j + 1, v)) {
        layer.SetTile(corners[LOWER_LEFT_CORNER], i, j + 1, z);
      }
      if (Empty(i - 1, j + 1, v) && Empty(i, j + 1, v) && Empty(i + 1, j + 1, v)) {
        layer.SetTile(corners[RIGHT_EDGE], i, j + 1, z);
      }
      //SW
      if (Filled(i + 1, j + 1, v) && Empty(i + 1, j, v) && Empty(i + 1, j - 1, v)) {
        layer.SetTile(corners[UPPER_RIGHT_CORNER], i + 1, j, z);
      }
      if (Empty(i + 1, j + 1, v) && Empty(i + 1, j, v) && Filled(i + 1, j - 1, v)) {
        layer.SetTile(corners[UPPER_LEFT_CORNER], i + 1, j, z);
      }
      if (Empty(i + 1, j + 1, v) && Empty(i + 1, j, v) && Empty(i + 1, j - 1, v)) {
        layer.SetTile(corners[DOWN_EDGE], i + 1, j, z);
      }
      //NW
      if (Filled(i + 1, j - 1, v) && Empty(i, j - 1, v) && Empty(i - 1, j - 1, v)) {
        layer.SetTile(corners[LOWER_RIGHT_CORNER], i, j - 1);
      }
      if (Empty(i + 1, j - 1, v) && Empty(i, j - 1, v) && Filled(i - 1, j - 1, v)) {
        layer.SetTile(corners[UPPER_RIGHT_CORNER], i, j - 1);
      }
      if (Empty(i + 1, j - 1, v) && Empty(i, j - 1, v) && Empty(i - 1, j - 1, v)) {
        layer.SetTile(corners[LEFT_EDGE], i, j - 1);
      }
      //N corner
      if (Empty(i, j - 1, v) && Empty(i - 1, j - 1, v) && Empty(i - 1, j, v)) {
        layer.SetTile(corners[UPPER_LEFT_EDGE], i - 1, j - 1);
      }
      ////E corner
      if (Empty(i - 1, j, v) && Empty(i - 1, j + 1, v) && Empty(i, j + 1, v)) {
        layer.SetTile(corners[UPPER_RIGHT_EDGE], i - 1, j + 1);
      }
      //S corner
      if (Empty(i, j + 1, v) && Empty(i + 1, j + 1, v) && Empty(i + 1, j, v)) {
        layer.SetTile(corners[LOWER_RIGHT_EDGE], i + 1, j + 1, z);
      }
      ////W corner
      if (Empty(i + 1, j, v) && Empty(i + 1, j - 1, v) && Empty(i, j - 1, v)) {
        layer.SetTile(corners[LOWER_LEFT_EDGE], i + 1, j - 1);
      }
    }

    /// <summary>
    /// Fills the region.
    /// </summary>
    /// <param name="regionFillCode">Region fill code.</param>
    /// <param name="group">Tile group.</param>
    /// <param name="targetLayer">Target layer.</param>
    public void FillRegion(int regionFillCode, SKTileGroup group, float densityFactor, int targetLayer)
    {
      if (targetLayer < 0 || targetLayer >= layers.Count)
        return;
      var layer = layers[targetLayer];
      var random = new Random(Guid.NewGuid().GetHashCode());
      var regionsToFill = regions.Where(x => x.RegionFill == regionFillCode);

      var defs = group.Rules
                      .SelectMany(a => a.TileDefinitions)
                      .SelectMany(b => b.Textures).ToList();
      
      regionsToFill.ForEach(x=>{
        var density = densityFactor * x.MaxCol;
        var filler = UniformFill.Fill(0, 0, x.MaxCol+1, x.MaxRow+1, density);

        //var texture = defs[random.Next(0, defs.Count())];
        //layer.SetTexture(texture, 4 * tileSize, 0 * tileSize);

        foreach (var item in filler) {
          var col = (int)item.X;
          var row = (int)item.Y;
          if(x[row, col]!=null){
            //the point is inside the region
            if(defs.Any()){
              var texture = defs[random.Next(0, defs.Count())];
              layer.SetTexture(texture, item.Y * tileSize, item.X * tileSize);
            }
          }
        }
      });
    }

    /// <summary>
    /// Creates the layer.
    /// </summary>
    /// <returns>The layer.</returns>
    private TileMapLayer CreateLayer(int zPos)
    {
      var layer = new TileMapLayer(this) {
        ZPosition = zPos,
        Position = new CGPoint(0, 0),
      };
      layers.Add(layer);
      this.AddChild(layer);
      return layer;
    }

    /// <summary>
    /// Flattens the layer.
    /// </summary>
    /// <returns>The layer.</returns>
    /// <param name="index">Index.</param>
    public SKSpriteNode FlattenLayer(int index)
    {
      if(index>=0 && index < layers.Count){
        return layers[index].FlattenLayer();
      }
      return null;
    }
  }
}
