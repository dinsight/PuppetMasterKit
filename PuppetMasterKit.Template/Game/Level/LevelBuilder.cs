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
using PuppetMasterKit.Ios.Tiles.Tilemap.Helpers;
using PuppetMasterKit.Terrain.Map;
using PuppetMasterKit.Terrain.Map.SimplePlacement;
using PuppetMasterKit.Utility.Subscript;
using Foundation;

namespace PuppetMasterKit.Template.Game.Level
{
  public class LevelBuilder
  {
    private SKScene scene;

    private Hud hudDisplay;

    private GameFlightMap flightMap;

    private ComponentSystem componentSystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Template.Game.LevelBuilder"/> class.
    /// </summary>
    /// <param name="scene">Scene.</param>
    /// <param name="componentSystem">Component system.</param>
    public LevelBuilder(SKScene scene, ComponentSystem componentSystem)
    {
      this.componentSystem = componentSystem;
      this.scene = scene;
      var size = scene.GetMapSize();

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
      AddHoles(data.Holes);
      return data;
    }

    /// <summary>
    /// Gets the frame.
    /// </summary>
    /// <returns>The frame.</returns>
    private Polygon GetMapFrame()
    {
      var frameRect = scene.GetMapSize();
      var frame = new Polygon(
        new Point(0, 0),
        new Point(0, (float)frameRect.Height),
        new Point((float)frameRect.Width, (float)frameRect.Height),
        new Point((float)frameRect.Width, 0)
      );

      return frame;
    }

    /// <summary>
    /// Adds the entities.
    /// </summary>
    private void AddEntities()
    {
      var frame = GetMapFrame();

      for (int i = 0; i < 1 ; i++) {
        var rabbit = RabbitBuilder.Build(componentSystem, frame);
        var agent = rabbit.GetComponent<Agent>();
        var random = new Random(Guid.NewGuid().GetHashCode());
        //var x = random.Next(10, 300);
        //var y = random.Next(100, 600);
        var x = 0;
        var y = 0;
        agent.Position = new Point(x, y);
        flightMap.AddHero(rabbit);
      }

      for (int i = 0; i < 0 ; i++) {
        var wolf = WolfBuilder.Build(componentSystem, frame);
        var agent = wolf.GetComponent<Agent>();
        var random = new Random(Guid.NewGuid().GetHashCode());
        var x = random.Next(10, 300);
        var y = random.Next(100, 600);
        agent.Position = new Point(x, y);
        flightMap.Add(wolf);
      }

      for (int i = 0; i < 0 ; i++) {
        var store = StoreBuilder.Build(componentSystem, frame);
        var agent = store.GetComponent<Agent>();
        var random = new Random(Guid.NewGuid().GetHashCode());
        var x = random.Next(10, 300);
        var y = random.Next(100, 600);
        agent.Position = new Point(x, y);
        flightMap.Add(store);
      }

      for (int i = 0; i < 1 ; i++) {
        var hole = HoleBuilder.Build(componentSystem, frame);
        var agent = hole.GetComponent<Agent>();
        var random = new Random(Guid.NewGuid().GetHashCode());
        var x = 250;
        var y = 395;
        agent.Position = new Point(x, y);
      } 
    }

    /// <summary>
    /// Adds the camera.
    /// </summary>
    public SKCameraNode AddCamera()
    {
      var frameRect = scene.GetFrame();
      var cameraNode = new SKCameraNode();

      cameraNode.XScale = 1.2f;
      cameraNode.YScale = 1.2f;
      //cameraNode.XScale = 5f;
      //cameraNode.YScale = 5f;
      var player = flightMap
        .GetHeroes()
        .Select(a => a.GetComponent<SpriteComponent>())
        .Select(s => s.Sprite.GetNativeSprite() as SKNode).First();

      player.AddChild(cameraNode);
      scene.Camera = cameraNode;
      return cameraNode;
    }

    /// <summary>
    /// Adds the hud.
    /// </summary>
    private void AddHud(SKCameraNode cameraNode)
    {
      var frameRect = scene.GetViewFrame();
      hudDisplay = Hud.Create("Hud", "control");
      cameraNode.AddChild(hudDisplay.Menu);

      var sz = new CGPoint(scene.View.Bounds.Width, scene.View.Bounds.Height);
      var s = scene.ConvertPointFromView(sz);
      var t = cameraNode.ConvertPointFromNode(s, scene);
      hudDisplay.Menu.Position = new CGPoint(-Math.Abs(t.X),-Math.Abs(t.Y) );

      Container.GetContainer().RegisterInstance<Hud>(hudDisplay);
    }

    /// <summary>
    /// Adds the holes.
    /// </summary>
    /// <param name="holes">Holes.</param>
    private void AddHoles(Entity[] holes)
    {
      var mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
      var frame = GetMapFrame();
      foreach (var item in holes) {
        var entity = HoleBuilder.Build(item, componentSystem, frame);
        flightMap.Add(entity);
      }
    }

    /// <summary>
    /// Build this instance.
    /// </summary>
    /// <returns>The build.</returns>
    public FlightMap Build()
    {
      AddEntities();
      var camera = AddCamera();
      var data = LoadSceneData();
      //scene.DrawObstacles(flightMap.Obstacles);
      //scene.DrawEnclosure();
      GenerateMap();
      AddHud(camera);
      return flightMap;
    }

    
    /// <summary>
    /// Generates the map1.
    /// </summary>
    private void GenerateMap()
    {
      var rows = 100;
      var cols = 100;
      var baseFolder = "/Users/alexjecu/Desktop/Workspace/dinsight/xamarin/assets/map";
      var existing = scene.Children.OfType<SKTileMapNode>();
      scene.RemoveChildren(existing.ToArray());

      var builder = Container.GetContainer().GetInstance<IMapGenerator>();
      var regions = builder.Create(rows, cols);
      //var mountains = regions.FirstOrDefault(x => x.Tiles.Count <= 0.05 * rows * cols);
      //if (mountains != null) {
      //  mountains.RegionFill = 4;
      //  mountains.Tiles.ToList().ForEach(x => mountains[x.Row, x.Col] = 4);
      //  PrintMap(mountains);
      //}
      //regions.Clear();
      //var reg = new Region(1);
      //reg.AddTile(0, 0);
      //regions.Add(reg);

      var mapping = new Dictionary<int, string> {
        //{ 1, "Grass"},
        { 1, "Marsh"},
        { 2, "Upland1"},
        { 0, "Dirt"},
        { 3, "Water" },
        { 4, "Marsh" }
      };

      var tileSize = 128;
      var s = new int[] { 0x0, 0x0, 0xff, 0xff };
      var e = new int[] { 0xAF, 0xFF, 0xFF, 0xee };

      var tileSet = SKTileSet.FromName("MainTileSet1");
      var f = tileSet.TileGroups.Where(x => x.Name == "Upland1").FirstOrDefault();

      var defaultPainter = new TiledRegionPainter(mapping, tileSet, randomSeed:1);
      var bicubicPainter = new BicubicRegionPainter(tileSize, s, e);
      var layeredPainter = new LayeredRegionPainter(1, new List<string>()
      { "Sand", "Water_L2", "Water", "Water_L1", "Water_L1",  }, tileSet, randomSeed: 0);
      var tileMap = new TileMap(defaultPainter, rows, cols, tileSize);
      tileMap.AddPainter(3, layeredPainter);

      Measure.Timed("Map building", () => {
        //tileMap.Build(regions, 0, '+', MapCodes.PATH, 'W', 1 );
        tileMap.Build(regions, 0,  1, 2, 3);
        //var woods = tileSet.TileGroups.First(x => x.Name == "Trees");
        //RegionFill.Fill(regions, tileSize, 1, woods, 0.001f, tileMap.GetLayer(0));
        var woods = tileSet.TileGroups.First(x => x.Name == "Marsh_Trees");
        RegionFill.Fill(regions, tileSize, 1, woods, 0.1f, tileMap.GetLayer(0), new Random(0));
      });

      
      var txt = SKTexture.FromImageNamed("M1");
      tileMap.GetLayer(0).SetTile(txt, 55, 50);

      var house = SKTexture.FromImageNamed("Hobbit");
      tileMap.GetLayer(0).SetTile(house, 65, 40);

      var granary = SKTexture.FromImageNamed("Granary");
      //var node = (SKSpriteNode)tileMap.GetLayer(0).SetTile(granary, 45, 38);
      var node = (SKSpriteNode)tileMap.GetLayer(0).SetTile(granary, 5, 10);
      //var shader = SKShader.FromFile("Wind.fsh");
      //node.Shader = shader;
      

      var hut = SKTexture.FromImageNamed("Hut");
      tileMap.GetLayer(0).SetTile(hut, 55, 30);
      tileMap.GetLayer(0).SetTile(hut, 5, 23);

      Measure.Timed("Dump image", () => {
        scene.AddChild(tileMap);
        //tileMap.FlattenLayer(0, x => x.SaveImage($"{baseFolder}/map0.png"));
      });

      PrintMap(builder);
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
