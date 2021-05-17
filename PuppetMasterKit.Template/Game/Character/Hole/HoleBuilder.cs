using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.Graphics.Geometry;
using LightInject;
using PuppetMasterKit.Ios.Tiles.Tilemap;
using static PuppetMasterKit.AI.Entity;
using static PuppetMasterKit.AI.Components.Agent;

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
    public static Entity Build(ComponentSystem componentSystem, Polygon boundaries, TileMap tileMap)
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

      entity = EntityBuilder.Builder(entity)
        .With(componentSystem,
              new StateComponent<HoleStates>(HoleStates.open),
              new SpriteComponent(CharacterName, new Size(25, 25)),
              new HealthComponent(100, 20, 3),
              new PhysicsComponent(5, 5, 1, 15),
              new CommandComponent() {
                OnEntityTouched = HoleHandlers.OnTouched,
                OnLocationTouched = HoleHandlers.OnMoveToPoint
              },
              AgentBuilder.Builder().Build())
        .WithName(CharacterName)
        .Build();

      return entity;
    }
  }
}
