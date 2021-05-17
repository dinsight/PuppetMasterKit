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
  public class StoreBuilder
  {
    private static string CharacterName = "store";

    private ComponentSystem componentSystem;
    private StoreStates initialState = StoreStates.full;
    private Polygon boundaries;
    private TileMap tileMap;
    private Point location;
    private FlightMap flightMap;

    public static StoreBuilder Builder(ComponentSystem componentSystem, StoreStates initialState = StoreStates.full) {
      var builder = new StoreBuilder {
        componentSystem = componentSystem,
        initialState = initialState
      };
      return builder;
    }

    public StoreBuilder WithBoundary(Polygon boundaries) {
      this.boundaries = boundaries;
      return this;
    }

    public StoreBuilder WithMap(TileMap tileMap)
    {
      this.tileMap = tileMap;
      return this;
    }

    public StoreBuilder AtLocation(Point location)
    {
      this.location = location;
      return this;
    }

    public StoreBuilder AtLocation(float x, float y)
    {
      this.location = new Point(x,y);
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
              new StateComponent<StoreStates>(initialState),
              new SpriteComponent(CharacterName, new Size(150, 150), new Point(0.5f,0.5f), null),
              new HealthComponent(100, 20, 3),
              new PhysicsComponent(5, 5, 1, 15),
              new CommandComponent() {
                OnEntityTouched = StoreHandlers.OnTouched,
                OnLocationTouched = StoreHandlers.OnMoveToPoint
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
