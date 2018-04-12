using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.Graphics.Geometry;
using LightInject;
using PuppetMasterKit.AI.Rules;

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
    public static Entity Build(ComponentSystem componentSystem)
    {
      var flightMap = Container.GetContainer().GetInstance<FlightMap>();

      var entity = EntityBuilder.Build()
        .With(componentSystem,
              new RuleSystemComponent<FlightMap, RabbitHandlers>(
                RabbitRulesBuilder.Build(flightMap), new RabbitHandlers()),
              new StateComponent<RabbitStates>(RabbitStates.idle),
              new SpriteComponent(CharacterName, new Size(30, 30)),
              new PhysicsComponent(10, 5, 1, 5),
              new CommandComponent(RabbitHandlers.OnTouched, RabbitHandlers.OnMoveToPoint),
              new Agent())
        .WithName(CharacterName)
        .GetEntity();
      
      entity.GetComponent<Agent>()
            .Add(new GoalToCohereWith(
              x => flightMap.GetAdjacentEntities(entity, p => p.Name == CharacterName), 150), 0.001f)
            .Add(new GoalToSeparateFrom(
              x => flightMap.GetAdjacentEntities(entity, p => p.Name == CharacterName), 50), 0.06f);
      
      return entity;
    }
  }
}
