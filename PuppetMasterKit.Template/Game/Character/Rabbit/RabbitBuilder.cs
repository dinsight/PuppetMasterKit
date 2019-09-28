using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.Graphics.Geometry;
using LightInject;
using PuppetMasterKit.AI.Rules;
using SpriteKit;

namespace PuppetMasterKit.Template.Game.Character.Rabbit
{
  public static class RabbitBuilder
  {
    private static string CharacterName = "rabbit";
    /// <summary>
    /// Build the specified componentSystem and flightMap.
    /// </summary>
    /// <returns>The build.</returns>
    /// <param name="componentSystem">Component system.</param>
    public static Entity Build(ComponentSystem componentSystem, Polygon boundaries)
    {
      var flightMap = Container.GetContainer().GetInstance<FlightMap>();

      var entity = EntityBuilder.Build()
        .With(componentSystem,
              //new RuleSystemComponent<FlightMap, RabbitHandlers>(
              //  RabbitRulesBuilder.Build(flightMap), new RabbitHandlers()),
              new StateComponent<RabbitStates>(RabbitStates.idle),
              new SpriteComponent(CharacterName, new Size(60, 60)),
              new HealthComponent(100, 20, 3),
              new PhysicsComponent(5, 30, 1, 30),
              new CommandComponent(RabbitHandlers.OnTouched, RabbitHandlers.OnMoveToPoint),
              new CollisionComponent((e) => 
                                     flightMap.GetAdjacentEntities(e, p => p.Name == "store" || p.Name == "hole"), 
                                     RabbitHandlers.HandleCollision, 35),
              new Agent())
        .WithName(CharacterName)
        .GetEntity();
      
      entity.GetComponent<Agent>()
          .Add(new GoalToCohereWith(x => flightMap.GetAdjacentEntities(entity, p => p.Name == CharacterName), 150), 0.001f)
          .Add(new GoalToSeparateFrom( x => flightMap.GetAdjacentEntities(entity, p => p.Name == CharacterName), 50), 0.005f)
          .Add(new ConstraintToStayWithin(boundaries))
          .Add(new GoalToAvoidObstacles(x => ((GameFlightMap)flightMap).GetObstacles(entity), 30));

      AddShadow(entity.GetComponent<SpriteComponent>().Sprite.GetNativeSprite() as SKSpriteNode);
      return entity;
    }

    /// <summary>
    /// Adds the shadow.
    /// </summary>
    /// <param name="node">Node.</param>
    private static void AddShadow(SKSpriteNode node)
    {
      //node.Shader = SKShader.FromFile("Shadow.fsh");
      var shadow = SKShapeNode.FromEllipse(size: new CoreGraphics.CGSize(20, 10));
      shadow.FillColor = UIKit.UIColor.Black;
      shadow.StrokeColor = UIKit.UIColor.FromRGBA(0, 0, 0, 0.1f);
      shadow.Alpha = 0.3f;
      shadow.Position = new CoreGraphics.CGPoint(0, 3);
      node.AnchorPoint = new CoreGraphics.CGPoint(0.5, 0);
      node.AddChild(shadow);
    }
  }
}
