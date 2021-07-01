using System;
using LightInject;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using PuppetMasterKit.AI;
using PuppetMasterKit.Ios.Tiles.Tilemap;
using PuppetMasterKit.Terrain.Map;
using PuppetMasterKit.Utility;
using SpriteKit;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.Template.Game.Controls;

namespace PuppetMasterKit.Template.Game
{
  public class BridgeBuilder
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="control"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static bool MultiSelectionValidator(PlotControl control, GridCoord prevPos, GridCoord pos)
    {
      var selected = ((PlotControl)control).GetSelectedTiles();
      //var canBuild = selected.Count > 0;//|| selected.Any(x => x.IsAdjacentTo(pos));
      if (prevPos != null) {
        if (pos.Row == prevPos.Row || pos.Col == prevPos.Col) {
          return true;
        }
        return false;
      }

      return true;
    }

    public static void Build(List<GridCoord> placementTiles, TileMap tileMap)
    {
      var layer = tileMap.GetLayer(1);

      var flightMap = Container.GetContainer().GetInstance<FlightMap>();
      var list = placementTiles.ToList();
      for (int index = 0; index < list.Count; index++) {
        var tile = list[index];
        SKTexture deckPiece = null;
        if (index > 0) {
          var prevTile = list[index - 1];
          if (prevTile.Col == tile.Col) {
            deckPiece = SKTexture.FromImageNamed("Deck_1");
          }

          if (prevTile.Row == tile.Row) {
            deckPiece = SKTexture.FromImageNamed("Deck_0");
          }

          if (index == 1) {
            layer.SetTile(deckPiece, prevTile.Col, prevTile.Row, null, new CGPoint(0.5, 0.5));
          }
          layer.SetTile(deckPiece, tile.Col, tile.Row, null, new CGPoint(0.5, 0.5));
        }

        flightMap.Board[tile.Col, tile.Row] = (int)Level.TerrainDefinition.TerrainType.DECK;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="placementTiles"></param>
    /// <param name="tileMap"></param>
    public static void BuildOld(List<GridCoord> placementTiles,TileMap tileMap)
    {
      var layer = tileMap.GetLayer(1);
      var region = new Region(0);
      placementTiles.ForEach(x =>
        region.AddTile(x.Col, x.Row)
      );

      var obstacles = new List<Obstacle>();
      var rad = tileMap.TileSize;

      if (!region.IsChain()) {
        var hud = Container.GetContainer().GetInstance<Hud>();
        hud.SetMessage("The bridge is not right");
        return;
      }

      var flightMap = Container.GetContainer().GetInstance<FlightMap>();
      region.TraverseChain(region.Tiles.ToList(), (r, c, type) => {
        SKTexture fence = null;
        if (type == TileType.LeftSide ||
            type == TileType.RightSide ||
            type == TileType.CulDeSacTop ||
            type == TileType.CulDeSacBottom) {
          fence = SKTexture.FromImageNamed("Deck_0");
        }
        if (type == TileType.TopSide ||
            type == TileType.BottomSide ||
            type == TileType.CulDeSacLeft ||
            type == TileType.CulDeSacRight) {
          fence = SKTexture.FromImageNamed("Deck_1");
        }

        if (fence != null) {
          layer.SetTile(fence, r, c, null, new CGPoint(0.5, 0.5));
        } else {
          var weird = fence;
        }

        var rowc = r * tileMap.TileSize + tileMap.TileSize / 2;
        var colc = c * tileMap.TileSize + tileMap.TileSize / 2;
        flightMap.Board[r, c] = (int)Level.TerrainDefinition.TerrainType.DECK;
      });
    }
  }
}
