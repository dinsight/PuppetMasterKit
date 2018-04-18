using System;
using System.Linq;
using CoreGraphics;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Template.Game.Character.Rabbit;
using PuppetMasterKit.Template.Game.Character.Wolf;
using PuppetMasterKit.Template.Game.Ios.Bindings;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template.Game
{
  public class LevelBuilder
  {
    private SKScene scene;

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
      var rect = scene.GetFrame();
      this.flightMap = new GameFlightMap((float)rect.Width, (float)rect.Height, 7, 7);
      Registration.RegisterBindings(scene);
      Registration.Register<FlightMap>(flightMap);
    }

    /// <summary>
    /// Adds the entities.
    /// </summary>
    private void AddEntities()
    {
      var frameRect = scene.GetFrame();
      var frame = new Polygon(
        new Point(0, 0),
        new Point(0, (float)frameRect.Height),
        new Point((float)frameRect.Width, (float)frameRect.Height),
        new Point((float)frameRect.Width, 0)
      );
      for (int i = 0; i < 1; i++) {
        var rabbit = RabbitBuilder.Build(componentSystem, frame);
        var theSprite = rabbit.GetComponent<SpriteComponent>()?.Sprite;
        var random = new Random(Guid.NewGuid().GetHashCode());
        var x = random.Next(10, 300);
        var y = random.Next(100, 600);
        theSprite.Position = new Point(x, y);
        flightMap.AddHero(rabbit);
      }

      for (int i = 0; i < 0; i++) {
        var wolf = WolfBuilder.Build(componentSystem, frame);
        var theSprite = wolf.GetComponent<SpriteComponent>()?.Sprite;
        var random = new Random(Guid.NewGuid().GetHashCode());
        var x = random.Next(10, 300);
        var y = random.Next(100, 600);
        theSprite.Position = new Point(x, y);
        flightMap.Add(wolf);
      }

      for (int i = 0; i < 1; i++) {
        var store = StoreBuilder.Build(componentSystem, frame);
        var theSprite = store.GetComponent<SpriteComponent>()?.Sprite;
        var random = new Random(Guid.NewGuid().GetHashCode());
        var x = random.Next(10, 300);
        var y = random.Next(100, 600);
        theSprite.Position = new Point(x, y);
        flightMap.Add(store);
      }
    }

    /// <summary>
    /// Adds the camera.
    /// </summary>
    public SKCameraNode AddCamera()
    {
      var frameRect = scene.GetFrame();
      var cameraNode = new SKCameraNode();

      cameraNode.XScale = 0.9f;
      cameraNode.YScale = 0.9f;
      var player = flightMap.GetHeroes()
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
      var scn = SKNode.FromFile<SKScene>("Hud");
      var menu = scn.Children.FirstOrDefault(x => x.Name == "control") as SKSpriteNode;
      menu.RemoveFromParent();
      cameraNode.AddChild(menu);


      var sz = new CGPoint(scene.View.Bounds.Width, scene.View.Bounds.Height);
      var s = scene.ConvertPointFromView(sz);
      var t = cameraNode.ConvertPointFromNode(s, scene);
      menu.Position = new CGPoint(-Math.Abs(t.X),-Math.Abs(t.Y) );
    }

    /// <summary>
    /// Build this instance.
    /// </summary>
    /// <returns>The build.</returns>
    public FlightMap Build()
    {
      AddEntities();
      AddHud(AddCamera());
      return flightMap;
    }
  }
}
