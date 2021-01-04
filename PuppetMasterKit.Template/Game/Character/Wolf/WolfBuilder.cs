using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.Graphics.Geometry;
using LightInject;
using PuppetMasterKit.Ios.Tiles.Tilemap;
using static PuppetMasterKit.AI.Entity;
using static PuppetMasterKit.AI.Components.Agent;

namespace PuppetMasterKit.Template.Game.Character.Wolf
{
  public static class WolfBuilder
  {
    private static string CharacterName = "wolf";
    /// <summary>
    /// Build the specified componentSystem and flightMap.
    /// </summary>
    /// <returns>The build.</returns>
    /// <param name="componentSystem">Component system.</param>
    public static Entity Build(ComponentSystem componentSystem, Polygon boundaries, TileMap tileMap)
    {
      var flightMap = Container.GetContainer().GetInstance<FlightMap>();

      var entity = EntityBuilder.Builder()
        .With(componentSystem,
              new RuleSystemComponent<FlightMap, WolfHandlers>(WolfRulesBuilder.Build(flightMap), new WolfHandlers()),
              new StateComponent<WolfStates>(WolfStates.idle),
              new UpdateableSpriteComponent(CharacterName, new Size(120, 140), new Point(0.5f, 0.2f)),
              new HealthComponent(100, 20, 3),
              new PhysicsComponent(5, 7, 1, 80),
              new CommandComponent(WolfHandlers.OnTouched, WolfHandlers.OnMoveToPoint),
              new CollisionComponent((e) => flightMap.GetAdjacentEntities(e, p => p.Name == "beaver"), WolfHandlers.WolfMeetsPrey, 80),
              AgentBuilder.Builder()
                .With(new GoalToCohereWith(x => flightMap.GetAdjacentEntities(x, p => p.Name == CharacterName), 150), 0.0003f)
                .With(new GoalToSeparateFrom(x => flightMap.GetAdjacentEntities(x, p => p.Name == CharacterName), 100), 0.18f)
                .With(new ConstraintToStayWithin(boundaries))
                .With(new GoalToAvoidObstacles(x => ((GameFlightMap)flightMap).GetObstacles(x)))
                .Build())
        .WithName(CharacterName)
        .Build();
      
      return entity;
    }
  }
}
