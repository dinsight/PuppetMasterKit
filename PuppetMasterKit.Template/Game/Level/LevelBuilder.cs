using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using LightInject;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.AI.Map;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Template.Game.Character.Rabbit;
using PuppetMasterKit.Template.Game.Character.Wolf;
using PuppetMasterKit.Ios.Isometric.Tilemap;
using SpriteKit;
using UIKit;
using PuppetMasterKit.Ios.Isometric.Fill;
using System.Diagnostics;

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

      for (int i = 0; i < 0 ; i++) {
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

    private void GenerateMap1()
    {
      var existing = scene.Children.OfType<SKTileMapNode>();
      scene.RemoveChildren(existing.ToArray());
      var map = new int[,]{
        {'W','W','W','W'},
        {'W','W','W','W'},
        {'W','W','W','W'},
        {'W','W','W','W'},

      };
      var mapping = new Dictionary<int, string> {
        { '-', "Cobblestone"},
        { '+', "Sand"},
        { '|', "Water" },
        { 'W', "GrassBk" },
      };

      var tileSize = 128;
      var tileSet = SKTileSet.FromName("Template Tile Set");
      var tileMap = new TileMap(map, mapping, tileSet, tileSize);
      tileMap.Build('-','+', 'W', '|');
      tileMap.Position = new CGPoint(0, 0);
      scene.AddChild(tileMap);
      var woods = tileSet.TileGroups.First(x => x.Name == "Wood");
      RegionFill.Fill(tileMap.Regions, tileSize, 'W', woods, 1, tileMap.GetLayer(0));
      var layer = tileMap.FlattenLayer(0);
    }

    private void GenerateMap()
    {
      var existing = scene.Children.OfType<SKTileMapNode>();
      scene.RemoveChildren(existing.ToArray());

      var modules = new List<Module>();

      var module0 = new Module(new int[,] {
                { 3,3,3,3,3},
                { 3,3,3,3,3},
                { 3,3,3,3,3},
                { 3,3,3,3,3},
      }, '+');

      var module1 = new Module(new int[,] {
                { 3,3,3,3,3,3,3,},
                { 3,3,3,3,3,3,3,},
                { 3,3,3,3,3,3,3,},
                { 3,3,3,3,3,3,3,},
                { 3,3,3,3,3,3,3,},
      }, 1){ IsAccessible = false };

      var module2 = new Module(new int[,] {
                { 0,0,0,3,0,0,0,0,0 },
                { 3,3,3,3,3,3,3,3,3 },
                { 3,3,3,3,3,3,3,3,3 },
                { 3,3,3,3,3,3,3,3,3 },
                { 3,3,3,3,3,3,3,3,3 },
                { 3,3,3,3,3,3,3,3,3 },
                { 3,3,3,3,3,3,3,3,3 },
            }, 'W');

      modules.Add(module0);
      modules.Add(module1);
      modules.Add(module2);

      var builder = new MapBuilder(105, 105, 5, new PathFinder());
      builder.Create(120, modules);


      var mapping = new Dictionary<int, string> {
        { '-', "Water" },
        { '+', "Sand" },
        { MapCodes.PATH, "Dirt" },
        { 1, "Water" },
        { 'W', "Grass"},
      };

      var tileSize = 128;
      var tileSet = SKTileSet.FromName("Template Tile Set");
      var tileMap = new TileMap(builder.Map, mapping, tileSet, tileSize);
      tileMap.Build('+', 1, 'W','|',MapCodes.PATH,'-');
      tileMap.Position = new CGPoint(0, 0);

      var woods = tileSet.TileGroups.First(x => x.Name == "Trees");
      var water = tileSet.TileGroups.First(x => x.Name == "Water");
      RegionFill.Fill(tileMap.Regions, tileSize, 'W', woods, 0.8f, tileMap.GetLayer(0));
      //RegionFill.Blend(tileMap.Regions, tileSize, 1, water, tileMap.GetLayer(2));
      scene.AddChild(tileMap);
      var start = DateTime.Now;
      var layer = tileMap.FlattenLayer(0);
      tileMap.FlattenLayer(2);
      var end = DateTime.Now;
      Debug.WriteLine($"Took: {(end-start).TotalMilliseconds} ms");
    }
  }
}
