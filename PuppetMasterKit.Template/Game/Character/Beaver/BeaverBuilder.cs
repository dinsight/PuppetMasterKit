using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.Graphics.Geometry;
using LightInject;
using SpriteKit;
using System.Collections.Generic;
using System;
using PuppetMasterKit.Ios.Tiles.Tilemap;
using static PuppetMasterKit.AI.Entity;
using static PuppetMasterKit.AI.Components.Agent;
using PuppetMasterKit.Template.Game.Controls;
using PuppetMasterKit.Graphics.Sprites;
using System.Linq;
using PuppetMasterKit.Utility;
using Pair = System.Tuple<int, int>;

namespace PuppetMasterKit.Template.Game.Character.Rabbit
{
  public static class BeaverBuilder
  {
    private static string CharacterName = "beaver";
    private static string RangeWeaponAtlas = "artifacts/rocks.atlas";
    private static string RangeWeaponName = "pebble.png";

    /// <summary>
    /// Build the specified componentSystem and flightMap.
    /// </summary>
    /// <returns>The build.</returns>
    /// <param name="componentSystem">Component system.</param>
    public static Entity Build(ComponentSystem componentSystem, Polygon boundaries, TileMap tileMap, SKScene scene)
    {
      var flightMap = Container.GetContainer().GetInstance<FlightMap>();
      var hud = Container.GetContainer().GetInstance<Hud>();
      var mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();

      var entity = EntityBuilder.Builder()
        .With(componentSystem,
              //new RuleSystemComponent<FlightMap, RabbitHandlers>(
              //  RabbitRulesBuilder.Build(flightMap), new RabbitHandlers()),
              new StateComponent<BeaverStates>(BeaverStates.idle),
              new UpdateableSpriteComponent(CharacterName, new Size(80, 100), new Point(0.5f, 0.2f)),
              new RangeWeaponComponent(GetRangeWeaponCollisions(flightMap), RangeWeaponAtlas, RangeWeaponName,
                new Size(30, 30), 700, 10, 500),
              new HealthComponent(100, 20, 3),
              new PhysicsComponent(5, 8, 1, 3, 1),
              new CommandComponent() {
                  OnEntityTouched = BeaverHandlers.OnTouched,
                  OnLocationTouched = BeaverHandlers.OnMoveToPoint,
                  OnAttackPoint = BeaverHandlers.OnAttackPoint
              },
              new CollisionComponent(GetCollisions(flightMap), BeaverHandlers.HandleCollision, 80),
              AgentBuilder.Builder()
                .With(new GoalToCohereWith(x => flightMap.GetAdjacentEntities(x, p => p.Name == CharacterName), 150), 0.001f)
                .With(new GoalToSeparateFrom(x => flightMap.GetAdjacentEntities(x, p => p.Name == CharacterName), 50), 0.005f)
                .With(new ConstraintToStayWithin(boundaries))
                .With(new GoalToAvoidObstacles(x => ((GameFlightMap)flightMap).GetObstacles(x), 30))
              .Build())
        .WithName(CharacterName)
        .Build();

      AddShadow(entity.GetComponent<SpriteComponent>().Sprite.GetNativeSprite() as SKSpriteNode);
      SetupMenu(componentSystem, boundaries, tileMap, scene, hud, mapper, entity);

      return entity;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="componentSystem"></param>
    /// <param name="boundaries"></param>
    /// <param name="tileMap"></param>
    /// <param name="scene"></param>
    /// <param name="hud"></param>
    /// <param name="mapper"></param>
    /// <param name="entity"></param>
    private static void SetupMenu(ComponentSystem componentSystem,
      Polygon boundaries, TileMap tileMap, SKScene scene, Hud hud, ICoordinateMapper mapper, Entity entity)
    {
      var plotter = PlotControl.CreateFromFile(scene, tileMap, "Hud", "plotter");
      plotter.SelectionValidator += MultiSelectionValidator;
      hud.OnShowMenu += (sender, gesture) => {
        plotter.Open(entity);
      };

      plotter.OnOk = (control, actionName) => {
        var selection = control.GetSelectedTiles();
        if (selection.Count == 0)
          return;

        var row = selection[0].Row;
        var col = selection[0].Col;
        var y = tileMap.TileSize * row + tileMap.TileSize / 2;
        var x = tileMap.TileSize * col + tileMap.TileSize / 2;
        var coord2d = mapper.ToScene(new Point(x, y));

        BeaverHandlers.GoToLocation(entity, coord2d, (agent, location) => {
          var state = entity.GetComponent<StateComponent<BeaverStates>>();
          state.CurrentState = BeaverStates.idle;

          if (actionName == "build_granary") {
            BeaverHandlers.OnBuildGranary(entity, Point.Zero, tileMap, componentSystem, boundaries);

          }
          if (actionName == "build_tower") {
            BeaverHandlers.OnBuildTower(entity, Point.Zero, tileMap, componentSystem, boundaries);
          }

          if (actionName == "build_fence") {
            BeaverHandlers.OnBuildFence(entity, control.GetSelectedTiles(), Point.Zero, tileMap, componentSystem, boundaries);
          }
          control.ClearSelection();
        });
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="control"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static bool MultiSelectionValidator(PlotControl control, GridCoord pos) {
      var selected = ((PlotControl)control).GetSelectedTiles();
      var canBuild = selected.Count==0 || selected.Any(x => x.IsAdjacentTo(pos));
      return canBuild;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="flightMap"></param>
    /// <returns></returns>
    static Func<Entity, IEnumerable<Entity>> GetCollisions(FlightMap flightMap)
    {
      return (e) =>flightMap.GetAdjacentEntities(e, p => p.Name == "store" || p.Name == "tower");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="flightMap"></param>
    /// <returns></returns>
    static Func<Entity, IEnumerable<Entity>> GetRangeWeaponCollisions(FlightMap flightMap)
    {
      return (e) => flightMap.GetAdjacentEntities(e, p => p.Name == "wolf" );
    }

    /// <summary>
    /// Adds the shadow.
    /// </summary>
    /// <param name="node">Node.</param>
    private static void AddShadow(SKSpriteNode node)
    {
      //node.Shader = SKShader.FromFile("Shadow.fsh");
      var shadow = SKShapeNode.FromEllipse(size: new CoreGraphics.CGSize(27, 10));
      shadow.FillColor = UIKit.UIColor.Black;
      shadow.StrokeColor = UIKit.UIColor.FromRGBA(0, 0, 0, 0.1f);
      shadow.Alpha = 0.3f;
      shadow.Position = new CoreGraphics.CGPoint(0, -10);
      node.AnchorPoint = new CoreGraphics.CGPoint(0.5, 0);
      node.AddChild(shadow);
    }
  }
}
