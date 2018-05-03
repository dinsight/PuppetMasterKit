﻿using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Template.Game.Character.Rabbit;
using PuppetMasterKit.Template.Game.Character.Wolf;
using PuppetMasterKit.Template.Game.Ios.Bindings;
using PuppetMasterKit.Utility;
using SceneKit;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template.Game
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
      for (int i = 0; i < 1 ; i++) {
        var rabbit = RabbitBuilder.Build(componentSystem, frame);
        var theSprite = rabbit.GetComponent<SpriteComponent>()?.Sprite;
        var random = new Random(Guid.NewGuid().GetHashCode());
        //var x = random.Next(10, 300);
        //var y = random.Next(100, 600);
        var x = 150;
        var y = 195;
        theSprite.Position = new Point(x, y);
        flightMap.AddHero(rabbit);
      }

      for (int i = 0; i < 0 ; i++) {
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
    /// Loads the scene data.
    /// </summary>
    private LevelData LoadSceneData()
    {
      var data = LevelData.Load("PuppetMasterKit.Template.Resources.GameScene.json");
      flightMap.Obstacles.AddRange(data.Obstacles);
      return data;
    }

    /// <summary>
    /// Debug the specified data.
    /// </summary>
    /// <returns>The debug.</returns>
    /// <param name="data">Data.</param>
    private void Debug(LevelData data)
    {
      scene.DrawObstacles(data);
      scene.DrawEnclosure();
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
      Debug(data);
      AddHud(camera);
      return flightMap;
    }
  }
}
