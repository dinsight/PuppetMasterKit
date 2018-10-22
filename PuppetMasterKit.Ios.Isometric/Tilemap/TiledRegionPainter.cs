using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.AI;
using SpriteKit;

namespace PuppetMasterKit.Ios.Isometric.Tilemap
{
  public class TiledRegionPainter : IRegionPainter
  {
    private Dictionary<int, string> tileMapping;
    private SKTileSet tileSet;
    private int[,] map;
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
    private bool Empty(int i, int j, int v) => !Filled(i, j, v);
    private bool Filled(int i, int j, int v) => IsValid(i, j) && map[i, j] == v;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Ios.Isometric.Tilemap.TiledRegionPainter"/> class.
    /// </summary>
    /// <param name="tileMapping">Tile mapping.</param>
    /// <param name="tileSet">Tile set.</param>
    /// <param name="map">Map.</param>
    public TiledRegionPainter(Dictionary<int, string> tileMapping, SKTileSet tileSet, int[,] map)
    {
      this.tileMapping = tileMapping;
      this.tileSet = tileSet;
      this.map = map;
    }

    /// <summary>
    /// Paint the specified region and layer.
    /// </summary>
    /// <param name="region">Region.</param>
    /// <param name="layer">Layer.</param>
    public void Paint(Region region, TileMapLayer layer)
    {
      var tileGroup = tileMapping
          .Where(k => k.Key == region.RegionFill)
          .Select(x => tileSet
                  .TileGroups
                  .FirstOrDefault(t => t.Name == x.Value))
          .FirstOrDefault();

      var corners = GetCorners(tileGroup);
      foreach (var tile in region.Tiles) {
        if (tileGroup != null) {
          //set the appropriate texture for the adjacent tiles
          SetAdjacentTiles(layer,
                           corners,
                           tile.Row,
                           tile.Col);
          //set the tile texture
          layer.SetTile(tileGroup.GetTexture(CENTER),
                                tile.Row,
                                tile.Col);
        }
      }
    }

    /// <summary>
    /// Gets the corners.
    /// </summary>
    /// <returns>The corners.</returns>
    /// <param name="tileGroup">Tile group.</param>
    private Dictionary<string, SKTexture> GetCorners(SKTileGroup tileGroup)
    {
      var dictionary = new Dictionary<string, SKTexture>();

      var maintTile = tileGroup.GetTexture(CENTER);
      dictionary.Add(LOWER_LEFT_CORNER, maintTile.BlendWithAlpha(tileGroup.GetTexture(LOWER_LEFT_CORNER)));
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
    /// <param name="corners">Corners.</param>
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
  }
}
