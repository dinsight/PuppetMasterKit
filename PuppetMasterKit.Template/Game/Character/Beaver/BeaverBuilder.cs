using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.Graphics.Geometry;
using LightInject;
using PuppetMasterKit.AI.Rules;
using SpriteKit;
using System.Collections.Generic;
using System;
using UIKit;

namespace PuppetMasterKit.Template.Game.Character.Rabbit
{
  public static class BeaverBuilder
  {
    private static string CharacterName = "beaver";
    private static string RangeWeaponAtlas = "artifacts/rocks.atlas";
    private static string RangeWeaponName = "pebble.png";

    /// <summary>
    /// Build the specified componentSystem and flightMap.
    /// </summary>
    /// <returns>The build.</returns>
    /// <param name="componentSystem">Component system.</param>
    public static Entity Build(ComponentSystem componentSystem, Polygon boundaries)
    {
      var flightMap = Container.GetContainer().GetInstance<FlightMap>();
      var hud = Container.GetContainer().GetInstance<Hud>();

      var entity = EntityBuilder.Build()
        .With(componentSystem,
              //new RuleSystemComponent<FlightMap, RabbitHandlers>(
              //  RabbitRulesBuilder.Build(flightMap), new RabbitHandlers()),
              new StateComponent<BeaverStates>(BeaverStates.idle),
              new SpriteComponent(CharacterName, new Size(100, 120), new Point(0.5f, 0.2f)),
              new RangeWeaponComponent(GetRangeWeaponCollisions(flightMap),
                    RangeWeaponAtlas, RangeWeaponName, new Size(30, 30), 700, 10, 500),
              new HealthComponent(100, 20, 3),
              new PhysicsComponent(5, 7, 1, 3, 1),
              new CommandComponent(BeaverHandlers.OnTouched,
                    BeaverHandlers.OnMoveToPoint,
                    BeaverHandlers.OnBuildGranary),
              new CollisionComponent(GetCollisions(flightMap),
                    BeaverHandlers.HandleCollision, 80),
              new Agent())
        .WithName(CharacterName)
        .GetEntity();

      entity.GetComponent<Agent>()
          .Add(new GoalToCohereWith(x => flightMap.GetAdjacentEntities(entity, p => p.Name == CharacterName), 150), 0.001f)
          .Add(new GoalToSeparateFrom(x => flightMap.GetAdjacentEntities(entity, p => p.Name == CharacterName), 50), 0.005f)
          .Add(new ConstraintToStayWithin(boundaries))
          .Add(new GoalToAvoidObstacles(x => ((GameFlightMap)flightMap).GetObstacles(entity), 30));

      AddShadow(entity.GetComponent<SpriteComponent>().Sprite.GetNativeSprite() as SKSpriteNode);

      hud.OnHudButtonClick += (sender, btnName) => {
        if (btnName == "build_granary") {
          BeaverHandlers.OnBuildGranary(entity, Point.Zero);
        }
      };

      return entity;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="flightMap"></param>
    /// <returns></returns>
    static Func<Entity, IEnumerable<Entity>> GetCollisions(FlightMap flightMap)
    {
      return (e) =>flightMap.GetAdjacentEntities(e, p => p.Name == "store" || p.Name == "hole");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="flightMap"></param>
    /// <returns></returns>
    static Func<Entity, IEnumerable<Entity>> GetRangeWeaponCollisions(FlightMap flightMap)
    {
      return (e) => flightMap.GetAdjacentEntities(e, p => p.Name == "wolf" );
    }

    /// <summary>
    /// Adds the shadow.
    /// </summary>
    /// <param name="node">Node.</param>
    private static void AddShadow(SKSpriteNode node)
    {
      //node.Shader = SKShader.FromFile("Shadow.fsh");
      var shadow = SKShapeNode.FromEllipse(size: new CoreGraphics.CGSize(27, 10));
      shadow.FillColor = UIKit.UIColor.Black;
      shadow.StrokeColor = UIKit.UIColor.FromRGBA(0, 0, 0, 0.1f);
      shadow.Alpha = 0.3f;
      shadow.Position = new CoreGraphics.CGPoint(0, -10);
      node.AnchorPoint = new CoreGraphics.CGPoint(0.5, 0);
      node.AddChild(shadow);
    }
  }
}
