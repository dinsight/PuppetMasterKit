using System;
using PuppetMasterKit.AI;
using LightInject;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Ios.Tiles.Tilemap;
using static PuppetMasterKit.AI.Entity;
using static PuppetMasterKit.AI.Components.Agent;
using PuppetMasterKit.Utility.Configuration;

namespace PuppetMasterKit.Template.Game.Character.Tower
{
  public class TowerBuilder
  {
    private static string CharacterName = "tower";

    private ComponentSystem componentSystem;
    private TowerStates initialState = TowerStates.ready;
    private Polygon boundaries;
    private TileMap tileMap;
    private Point location;
    private FlightMap flightMap;

    public static TowerBuilder Builder(ComponentSystem componentSystem, TowerStates initialState = TowerStates.building)
    {
      var builder = new TowerBuilder {
        componentSystem = componentSystem,
        initialState = initialState
      };
      return builder;
    }

    public TowerBuilder WithBoundary(Polygon boundaries)
    {
      this.boundaries = boundaries;
      return this;
    }

    public TowerBuilder WithMap(TileMap tileMap)
    {
      this.tileMap = tileMap;
      return this;
    }

    public TowerBuilder AtLocation(Point location)
    {
      this.location = location;
      return this;
    }

    public TowerBuilder AtLocation(float x, float y)
    {
      this.location = new Point(x, y);
      return this;
    }

    /// <summary>
    /// Build the specified componentSystem and flightMap.
    /// </summary>
    /// <returns>The build.</returns>
    /// <param name="componentSystem">Component system.</param>
    public Entity Build()
    {
      var flightMap = Container.GetContainer().GetInstance<FlightMap>();

      var entity = EntityBuilder.Builder()
        .With(componentSystem,
              new StateComponent<TowerStates>(initialState),
              new SpriteComponent(CharacterName, new Size(150, 150), new Point(0.5f, 0.5f), null),
              new HealthComponent(100, 20, 3),
              new PhysicsComponent(5, 5, 1, 15),
              new CommandComponent(TowerHandlers.OnTouched, null),
              AgentBuilder.Builder()
                .AtLocation(location)
              .Build())
        .WithName(CharacterName)
        .Build();

      var sprite = entity.GetComponent<SpriteComponent>();

      flightMap.Add(entity);

      return entity;
    }
  }
}
