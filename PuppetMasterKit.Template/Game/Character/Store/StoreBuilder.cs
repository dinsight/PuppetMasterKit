using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.Graphics.Geometry;
using LightInject;
using PuppetMasterKit.AI.Rules;

namespace PuppetMasterKit.Template.Game.Character.Rabbit
{
  public static class StoreBuilder
  {
    private static string CharacterName = "store";
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
              new StateComponent<StoreStates>(StoreStates.full),
              new SpriteComponent(CharacterName, new Size(65, 65)),
              new HealthComponent(100, 20, 3),
              new PhysicsComponent(5, 5, 1, 15),
              new CommandComponent(StoreHandlers.OnTouched, StoreHandlers.OnMoveToPoint),
              new Agent())
        .WithName(CharacterName)
        .GetEntity();
      
      return entity;
    }
  }
}
