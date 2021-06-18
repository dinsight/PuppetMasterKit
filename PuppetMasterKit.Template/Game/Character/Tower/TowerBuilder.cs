using System;
using PuppetMasterKit.AI;
using LightInject;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Ios.Tiles.Tilemap;
using static PuppetMasterKit.AI.Entity;
using static PuppetMasterKit.AI.Components.Agent;
using PuppetMasterKit.Utility.Configuration;
using System.Collections.Generic;

namespace PuppetMasterKit.Template.Game.Character.Tower
{
  public class TowerBuilder
  {
    private static string CharacterName = "tower";
    private static string RangeWeaponAtlas = "artifacts/rocks.atlas";
    private static string RangeWeaponName = "pebble.png";

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
              new RuleSystemComponent<FlightMap, TowerHandlers>(TowerRulesBuilder.Build(flightMap), new TowerHandlers(), TimeSpan.FromSeconds(3)),
              new StateComponent<TowerStates>(initialState),
              new SpriteComponent(CharacterName, new Size(110, 110), new Point(0.5f, 0.25f), null),
              new HealthComponent(100, 20, 3),
              new RangeWeaponComponent(GetRangeWeaponCollisions(flightMap), RangeWeaponAtlas, RangeWeaponName,
                new Size(30, 30), 700, 10, 500),
              new PhysicsComponent(5, 5, 1, 15),
              new CommandComponent() {
                OnEntityTouched = TowerHandlers.OnTouched
              },
              AgentBuilder.Builder()
                .AtLocation(location)
              .Build())
        .WithName(CharacterName)
        .Build();

      var sprite = entity.GetComponent<SpriteComponent>();

      flightMap.Add(entity);

      return entity;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="flightMap"></param>
    /// <returns></returns>
    static Func<Entity, IEnumerable<Entity>> GetRangeWeaponCollisions(FlightMap flightMap)
    {
      return (e) => flightMap.GetAdjacentEntities(e, p => p.Name == "wolf");
    }
  }
}
