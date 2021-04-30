using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using LightInject;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Template.Game.Character.Rabbit;
using PuppetMasterKit.Template.Game.Character.Wolf;
using SpriteKit;
using PuppetMasterKit.Utility.Diagnostics;
using PuppetMasterKit.Ios.Tiles.Tilemap.Painters;
using PuppetMasterKit.Ios.Tiles.Tilemap;
using PuppetMasterKit.Terrain.Map;
using PuppetMasterKit.Utility.Subscript;
using UIKit;

namespace PuppetMasterKit.Template.Game.Level
{
  public class LevelBuilder
  {
    private const int LAYER_COUNT = 3;

    private SKScene scene;

    private Hud hudDisplay;

    private GameFlightMap flightMap;

    private ComponentSystem componentSystem;

    int  mapRows = 100;
    int  mapCols = 100;
    int  tileSize = 64;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Template.Game.LevelBuilder"/> class.
    /// </summary>
    /// <param name="scene">Scene.</param>
    /// <param name="componentSystem">Component system.</param>
    public LevelBuilder(SKScene scene, ComponentSystem componentSystem)
    {
      this.componentSystem = componentSystem;
      this.scene = scene;
      var size = new Size(2 * mapRows * tileSize, 2 * mapCols * tileSize);

      this.flightMap = new GameFlightMap(size.Width, size.Height, 7, 7);
      Ios.Bindings.Registration.RegisterBindings(scene);
      Ios.Bindings.Registration.Register<FlightMap>(flightMap);
    }

    /// <summary>
    /// Loads the obstacles.
    /// </summary>
    /// <returns>The obstacles.</returns>
    private Obstacle[] LoadObstacles()
    {      
      var mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
      var obstacles = scene.Children
           .Where(x => x.Name == "obstacle").OfType<SKSpriteNode>()
           .Select(a=> new CircularObstacle( mapper.FromScene(
                       new Point((float)a.Position.X, 
                                 (float)a.Position.Y)), 
                                 (float)a.Size.Width/2 ))
           .ToArray();
      return obstacles;
    }

    /// <summary>
    /// Loads the obstacles from map.
    /// </summary>
    /// <returns>The obstacles from map.</returns>
    private Obstacle[] LoadObstaclesFromMap()
    {
      var mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
      var obstacleMap = scene.Children.OfType<SKTileMapNode>()
                             .Where(x => x.Name == "Obstacles").FirstOrDefault();

      var map = new Dictionary<Tuple<int, int>, PolygonalObstacle>();
      if(obstacleMap!=null){
        var width = (float)obstacleMap.TileSize.Width;
        var height = (float)obstacleMap.TileSize.Height;
        for (nuint row = 0; row < obstacleMap.NumberOfRows; row++) {
          for (nuint col = 0; col < obstacleMap.NumberOfColumns; col++) {
            var tile = obstacleMap.GetTileDefinition(col,row);
            if(tile!=null){
              float dx = (obstacleMap.NumberOfRows - row - 1) * width;
              float dy = col * width;
              var obstacle =
                new PolygonalObstacle(
                  new Point(dx, dy),
                  new Point(dx + width, dy),
                  new Point(dx + width, dy + width),
                  new Point(dx, dy + width));

              map.Add(Tuple.Create((int)row, (int)col), obstacle);
            }
          }
        }
      }
      return ObstacleCluster.CreateClusters(map);
    }

    /// <summary>
    /// Loads the scene data.
    /// </summary>
    private LevelData LoadSceneData()
    {
      var data = LevelData.Load("PuppetMasterKit.Template.Resources.GameScene.json");
      flightMap.Obstacles.AddRange(LoadObstaclesFromMap());
      //AddHoles(data.Holes);
      return data;
    }

    /// <summary>
    /// Adds the entities.
    /// </summary>
    private void AddEntities(TileMap tileMap)
    {
      var frame = new Polygon(
        new Point(0, 0),
        new Point(0, (float)2 * mapRows* tileSize),
        new Point((float)2 * mapCols*tileSize, (float)mapRows * tileSize * 2),
        new Point((float)2 * mapCols*tileSize, 0)
      );

      for (int i = 0; i < 1 ; i++) {
        var beaver = BeaverBuilder.Build(componentSystem, frame, tileMap, this.scene);
        var agent = beaver.GetComponent<Agent>();
        var random = new Random(Guid.NewGuid().GetHashCode());
        //var x = random.Next(10, 300);
        //var y = random.Next(100, 600);
        var x = mapCols * tileSize/2;
        var y = mapRows * tileSize/2;
        agent.Position = new Point(x, y);
        flightMap.AddHero(beaver);
      }

      for (int i = 0; i < 2 ; i++) {
        var wolf = WolfBuilder.Build(componentSystem, frame, tileMap);
        var agent = wolf.GetComponent<Agent>();
        var random = new Random(Guid.NewGuid().GetHashCode());
        var x = random.Next(10, 300);
        var y = random.Next(100, 600);
        agent.Position = new Point(x, y);
        flightMap.Add(wolf);
      }

      for (int i = 0; i < 1 ; i++) {
        StoreBuilder.Builder(componentSystem, StoreStates.full)
          .WithBoundary(frame)
          .WithMap(tileMap)
          .AtLocation(300, 500)
          .Build();
      }

      for (int i = 0; i < 0 ; i++) {
        var hole = HoleBuilder.Build(componentSystem, frame, tileMap);
        var agent = hole.GetComponent<Agent>();
        var x = 250;
        var y = 395;
        agent.Position = new Point(x, y);
        flightMap.Add(hole);
      } 
    }

    /// <summary>
    /// Adds the camera.
    /// </summary>
    public SKCameraNode AddCamera()
    {
      var frameRect = scene.GetFrame();
      var cameraNode = new SKCameraNode();

      cameraNode.XScale = 1.5f;
      cameraNode.YScale = 1.5f;
      scene.Camera = cameraNode;
      return cameraNode;
    }

    /// <summary>
    /// Adds the hud.
    /// </summary>
    private void AddHud(SKCameraNode cameraNode)
    {
      hudDisplay = Hud.Create("Hud", "control");
      cameraNode.AddChild(hudDisplay);
      hudDisplay.UpdateLayout(scene, cameraNode);
    }

    /// <summary>
    /// Build this instance.
    /// </summary>
    /// <returns>The build.</returns>
    public FlightMap Build()
    {
      var camera = AddCamera();
      AddHud(camera);
      var data = LoadSceneData();
      //scene.DrawObstacles(flightMap.Obstacles);
      //scene.DrawEnclosure();
      var tileMap = GenerateMap();

      AddEntities(tileMap);
      var player = flightMap
        .GetHeroes()
        .Select(a => a.GetComponent<SpriteComponent>())
        .Select(s => s.Sprite.GetNativeSprite() as SKNode).First();

      player.AddChild(camera);
      //scene.AddChild(camera);
      return flightMap;
    }


    /// <summary>
    /// Generates the map1.
    /// </summary>
    private TileMap GenerateMap()
    {
      var existing = scene.Children.OfType<SKTileMapNode>();
      scene.RemoveChildren(existing.ToArray());

      var builder = Container.GetContainer().GetInstance<IMapGenerator>();
      var regions = builder.Create(mapRows, mapCols);

      var mapping = new Dictionary<int, string> {
        //{ 1, "Marsh"},
        { 3, "Upland1"},
        { 1, "Wang_Swamp"},
        { 0, "Wang_Dirt"},
        { 2, "Water" }
      };

      var tileSize = 128;
      var s = new int[] { 0x0, 0x0, 0xff, 0xff };
      var e = new int[] { 0xAF, 0xFF, 0xFF, 0xee };

      var tileSet = SKTileSet.FromName("MainTileSet1");

      //var bicubicPainter = new BicubicRegionPainter(tileSize, s, e);
      //var layeredPainter = new LayeredRegionPainter(1, new List<string>()
      //{ "Sand", "Water_L2", "Water", "Water_L1", "Water_L1",  }, tileSet, randomSeed: 0);
      var defaultPainter = new TiledRegionPainter(mapping, tileSet, randomSeed: 1);
      var tileMap = new TileMap(defaultPainter, mapRows, mapCols, tileSize);
      //tileMap.AddPainter(3, layeredPainter);

      Measure.Timed("Map building", () => {
        //tileMap.Build(regions, 0, '+', MapCodes.PATH, 'W', 1 );
        tileMap.Build(LAYER_COUNT, regions, 0,  1, 2, 3);
        var woods = tileSet.TileGroups.First(x => x.Name == "Marsh_Trees");
        RegionFill.Fill(regions, tileSize, 1, woods, 0.1f, tileMap.GetLayer(1), new Random(0));
      });

      
      //var txt = SKTexture.FromImageNamed("M1");
      //tileMap.GetLayer(1).SetTile(txt, 55, 50);

      //var house = SKTexture.FromImageNamed("Hobbit");
      //tileMap.GetLayer(1).SetTile(house, 65, 40);

      //var granary = SKTexture.FromImageNamed("Granary");
      //var node = (SKSpriteNode)tileMap.GetLayer(0).SetTile(granary, 45, 38);
      //var node = (SKSpriteNode)tileMap.GetLayer(0).SetTile(granary, 5, 10);
      //var shader = SKShader.FromFile("Wind.fsh");
      //node.Shader = shader;
      

      //var hut = SKTexture.FromImageNamed("Hut");
      //tileMap.GetLayer(1).SetTile(hut, 55, 30);
      //tileMap.GetLayer(1).SetTile(hut, 5, 23);

      Measure.Timed("Dump image", () => {
        scene.AddChild(tileMap);
        //tileMap.FlattenLayer(0, x => x.SaveImage($"/Users/alexjecu/Desktop/Workspace/dinsight/xamarin/tmp/map0.png"));
      });

      PrintMap(builder);

      return tileMap;
    }

    private static void PrintMap(I2DSubscript<int?> i2)
    {
      for (int i = 0; i < i2.Rows; i++) {
        for (int j = 0; j < i2.Cols; j++) {
          var x = i2[i, j];
          if (!x.HasValue) {
            Console.Write(" ");
          } else if (x == 0) {
            Console.Write("∙");
          } else if (x == 1) {
            Console.Write("#");
          } else if (x == 2) {
            Console.Write("^");
          } else if (x == 3) {
            Console.Write("~");
          } else {
            Console.Write((char)x);
          }
        }
        Console.WriteLine();
      }
    }
  }
}
