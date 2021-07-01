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

namespace PuppetMasterKit.Template.Game.Character.Fishery
{
  public class FisheryBuilder
  {
    private static string CharacterName = "fishery";

    private ComponentSystem componentSystem;
    private FisheryStates initialState = FisheryStates.ready;
    private Polygon boundaries;
    private TileMap tileMap;
    private Point location;
    private FlightMap flightMap;

    public static FisheryBuilder Builder(ComponentSystem componentSystem, FisheryStates initialState = FisheryStates.building)
    {
      var builder = new FisheryBuilder {
        componentSystem = componentSystem,
        initialState = initialState
      };
      return builder;
    }

    public FisheryBuilder WithBoundary(Polygon boundaries)
    {
      this.boundaries = boundaries;
      return this;
    }

    public FisheryBuilder WithMap(TileMap tileMap)
    {
      this.tileMap = tileMap;
      return this;
    }

    public FisheryBuilder AtLocation(Point location)
    {
      this.location = location;
      return this;
    }

    public FisheryBuilder AtLocation(float x, float y)
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
              new RuleSystemComponent<FlightMap, FisheryHandlers>(FisheryRulesBuilder.Build(flightMap), new FisheryHandlers(), TimeSpan.FromSeconds(10)),
              new StateComponent<FisheryStates>(initialState),
              new SpriteComponent(CharacterName, new Size(110, 110), new Point(0.5f, 0.25f), null),
              new HealthComponent(100, 20, 3),
              new PhysicsComponent(5, 5, 1, 15),
              new CommandComponent() {
                OnEntityTouched = FisheryHandlers.OnTouched
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

  }
}
