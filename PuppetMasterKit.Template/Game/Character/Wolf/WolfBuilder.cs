using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.Graphics.Geometry;
using LightInject;
using PuppetMasterKit.AI.Rules;

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
    public static Entity Build(ComponentSystem componentSystem, Polygon boundaries)
    {
      var flightMap = Container.GetContainer().GetInstance<FlightMap>();

      var entity = EntityBuilder.Build()
        .With(componentSystem,
              new RuleSystemComponent<FlightMap, WolfHandlers>(
                WolfRulesBuilder.Build(flightMap), new WolfHandlers()),
              new StateComponent<WolfStates>(WolfStates.idle),
              new SpriteComponent(CharacterName, new Size(30, 30)),
              new PhysicsComponent(75, 1, 1, 5),
              new CommandComponent(WolfHandlers.OnTouched, WolfHandlers.OnMoveToPoint),
              new Agent())
        .WithName(CharacterName)
        .GetEntity();
      
      entity.GetComponent<Agent>()
        .Add(new GoalToCohereWith(x => flightMap
            .GetAdjacentEntities(entity, p => p.Name == CharacterName), 150), 0.001f)
        .Add(new GoalToSeparateFrom(x => flightMap
            .GetAdjacentEntities(entity, p => p.Name == CharacterName), 50), 0.01f)
        .Add(new ConstraintToStayWithin(boundaries));
      
      return entity;
    }
  }
}
