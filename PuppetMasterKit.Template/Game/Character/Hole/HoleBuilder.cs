using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.Graphics.Geometry;
using LightInject;
using PuppetMasterKit.AI.Rules;

namespace PuppetMasterKit.Template.Game.Character.Rabbit
{
  public static class HoleBuilder
  {
    private static string CharacterName = "hole";
    /// <summary>
    /// Build the specified componentSystem and flightMap.
    /// </summary>
    /// <returns>The build.</returns>
    /// <param name="componentSystem">Component system.</param>
    public static Entity Build(ComponentSystem componentSystem, Polygon boundaries)
    {
      return Build(null, componentSystem, boundaries);
    }

    /// <summary>
    /// Build the specified entity, componentSystem and boundaries.
    /// </summary>
    /// <returns>The build.</returns>
    /// <param name="entity">Entity.</param>
    /// <param name="componentSystem">Component system.</param>
    /// <param name="boundaries">Boundaries.</param>
    public static Entity Build(Entity entity, ComponentSystem componentSystem, Polygon boundaries)
    {
      var flightMap = Container.GetContainer().GetInstance<FlightMap>();

      entity = EntityBuilder.Build(entity)
        .With(componentSystem,
              new StateComponent<HoleStates>(HoleStates.open),
              new SpriteComponent(CharacterName, new Size(65, 65)),
              new HealthComponent(100, 20, 3),
              new PhysicsComponent(5, 5, 1, 15),
              new CommandComponent(HoleHandlers.OnTouched, HoleHandlers.OnMoveToPoint),
              new Agent())
        .WithName(CharacterName)
        .GetEntity();

      return entity;
    }
  }
}
