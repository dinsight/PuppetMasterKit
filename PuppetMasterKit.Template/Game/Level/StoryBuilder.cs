using System;
using System.Linq;
using LightInject;
using PuppetMasterKit.Ios.Tiles.Tilemap;
using PuppetMasterKit.Utility.Extensions;
using SpriteKit;
using PuppetMasterKit.Utility.Configuration;
using static PuppetMasterKit.Template.Game.Level.TerrainDefinition;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Graphics.Geometry;
using CoreGraphics;

namespace PuppetMasterKit.Template.Game.Level
{
  public class StoryBuilder
  {
    GameFlightMap flightMap;
    TileMap       tileMap;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="flightMap"></param>
    /// <param name="tileMap"></param>
    public StoryBuilder(GameFlightMap flightMap, TileMap tileMap)
    {
      this.flightMap = flightMap;
      this.tileMap = tileMap;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public StoryBuilder WithFishingSpots(int count) {
      var waterTiles = flightMap.Board.GetEnumerable()
        .Where((element) => element.Value == (int)TerrainType.WATER)
        .ToList();

      var mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
      var random = new Random(1);
      var index = 0;
      do {
        var pos = random.Next(10, 100);
        index += pos;
        if (index >= waterTiles.Count)
          break;
        var tile = waterTiles[index];
        flightMap.Board[tile.Row, tile.Col] = (int)TerrainType.FISHING_SPOT;

        var layer0 = tileMap.GetLayer(0);
        var layer1 = tileMap.GetLayer(1);

        var x = (tile.Row + 1) * tileMap.TileSize;
        var y = (tile.Col + 1) * tileMap.TileSize;

        var effect = SKEmitterNode.FromFile<SKEmitterNode>("FishJumpEffect");
        var fishPos = mapper.ToScene(new Point(x - tileMap.TileSize / 2, y - tileMap.TileSize / 2));
        effect.Position = new CGPoint(fishPos.X, fishPos.Y);
        effect.RunAction(
        SKAction.RepeatActionForever(
          SKAction.Sequence(
            SKAction.WaitForDuration(5),
            SKAction.Run(() => effect.ResetSimulation())
            )));
        layer1.AddChild(effect);

      } while (true);

      return this;
    }
    
  }
}
