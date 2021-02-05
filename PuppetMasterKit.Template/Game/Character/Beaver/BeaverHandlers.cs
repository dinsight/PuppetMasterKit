using System;
using LightInject;
using System.Linq;
using System.Diagnostics;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.AI.Rules;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Template.Game.Facts;
using PuppetMasterKit.Template.Game.Components;
using System.Collections.Generic;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;
using PuppetMasterKit.Ios.Tiles.Tilemap.Helpers;
using PuppetMasterKit.Ios.Tiles.Tilemap;
using PuppetMasterKit.Template.Game.Character.Tower;

namespace PuppetMasterKit.Template.Game.Character.Rabbit
{
  public class BeaverHandlers : FactHandler
  {
    /// <summary>
    /// Ons the touched.
    /// </summary>
    /// <param name="rabbit">Rabbit.</param>
    public static void OnTouched(Entity rabbit)
    {
      var state = rabbit.GetComponent<StateComponent>();
      if (state != null) {
        state.IsSelected = !state.IsSelected;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static void OnBuildGranary(Entity entity, Point location,
      TileMap tileMap,
      ComponentSystem componentSystem,
      Polygon boundaries)
    {
      Debug.WriteLine("Building the granary...");
      var agent = entity.GetComponent<Agent>();
      var state = entity.GetComponent<StateComponent<BeaverStates>>();
      agent.Remove<GoalToFollowPath>();
      state.CurrentState = BeaverStates.build;
      StoreBuilder.Builder(componentSystem, StoreStates.building)
          .AtLocation(agent.Position.X, agent.Position.Y)
          .Build();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="location"></param>
    /// <param name="tileMap"></param>
    /// <param name="componentSystem"></param>
    /// <param name="boundaries"></param>
    public static void OnBuildTower(Entity entity, Point location,
      TileMap tileMap,
      ComponentSystem componentSystem,
      Polygon boundaries)
    {
      Debug.WriteLine("Building tower...");
      var agent = entity.GetComponent<Agent>();
      var state = entity.GetComponent<StateComponent<BeaverStates>>();
      agent.Remove<GoalToFollowPath>();
      state.CurrentState = BeaverStates.build;
      TowerBuilder.Builder(componentSystem, TowerStates.building)
          .AtLocation(agent.Position.X, agent.Position.Y)
          .Build();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="zero"></param>
    /// <param name="tileMap"></param>
    /// <param name="componentSystem"></param>
    /// <param name="boundaries"></param>
    /// <param name="tuples"></param>
    internal static void OnBuildFence(Entity entity,
      Point zero,
      TileMap tileMap,
      ComponentSystem componentSystem,
      Polygon boundaries,
      List<Tuple<int,int>> tuples)
    {
      var flightMap = Container.GetContainer().GetInstance<FlightMap>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="location"></param>
    public static void OnAttackPoint(Entity entity, Point location)
    {
      var mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
      var weapon = entity.GetComponent<RangeWeaponComponent>();
      var agent = entity.GetComponent<Agent>();
      Point point = mapper.FromScene(location);
      weapon.Fire(point);
    }

    /// <summary>
    /// Ons the move to point.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="location">Location.</param>
    public static void OnMoveToPoint(Entity entity, Point location)
    {
      var hud = Container.GetContainer().GetInstance<Hud>();
      if (hud.IsControlPressed) {
        OnAttackPoint(entity, location);
        return;
      }
      var agent = entity.GetComponent<Agent>();
      if (agent == null)
        return;

      var mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
      var flightMap = Container.GetContainer().GetInstance<FlightMap>() as GameFlightMap;

      var state = entity.GetComponent<StateComponent<BeaverStates>>();
      state.CurrentState = BeaverStates.walk;
      //remove existing follow path command
      agent.Remove<GoalToFollowPath>();

      var obstacles = flightMap.Obstacles.OfType<PolygonalObstacle>().ToList();
      var newPath = ObstaclePath.GetPathTroughObstacles(obstacles, agent.Position, mapper.FromScene(location));

      //Container.GetContainer().GetInstance<SKScene>().DrawPath(newPath);
      //create new goal. Makes sure the goal is deleted upon arrival
      var goToPoint = new GoalToFollowPath(newPath.ToArray(), 10).WhenArrived((x, p) => {
        state.CurrentState = BeaverStates.idle;
      });
      agent.Add(goToPoint, 3);
    }

    /// <summary>
    /// Handle the specified target and fact.
    /// </summary>
    /// <returns>The handle.</returns>
    /// <param name="target">Target.</param>
    /// <param name="fact">Fact.</param>
    public void Handle(Entity target, Hungry fact)
    {
      Debug.WriteLine("Me Hungry...");
    }

    /// <summary>
    /// Handle the specified rabbit, entity and state.
    /// </summary>
    /// <returns>The handle.</returns>
    /// <param name="beaver">Rabbit.</param>
    /// <param name="entity">Entity.</param>
    /// <param name="state">State.</param>
    public static void HandleCollision(Entity beaver,
                              Entity entity,
                              CollisionState state)
    {
      if (entity.Name == "tower") {
        var towerState = entity.GetComponent<StateComponent<TowerStates>>();
        var beaverState = beaver.GetComponent<StateComponent<BeaverStates>>();
        if (state.Status == CollisionStatus.IN_PROGRESS && state.StopWatchValue > 20) {
          towerState.CurrentState = TowerStates.ready;
          beaverState.CurrentState = BeaverStates.idle;
          var towerSprite = entity.GetComponent<SpriteComponent>();
          towerSprite.OnSetEntity();
        }
      }
      if (entity.Name == "store") {
        var storeState = entity.GetComponent<StateComponent<StoreStates>>();

        var storeAgent = entity.GetComponent<Agent>();
        var beaverAgent = beaver.GetComponent<Agent>();
        var beaverState = beaver.GetComponent<StateComponent<BeaverStates>>();
        if (state.Status == CollisionStatus.INIT) {
          beaverAgent.Remove<GoalToFollowPath>();
          if (storeState.CurrentState == StoreStates.building) {
            beaverState.CurrentState = BeaverStates.build;
          } else {
            beaverState.CurrentState = BeaverStates.chop;
            GatherFood(beaver, entity, state);
          }
        }

        if (state.Status == CollisionStatus.IN_PROGRESS && state.StopWatchValue > 20) {
          storeState.CurrentState = StoreStates.full;
          beaverState.CurrentState = BeaverStates.idle;
          var storeSprite = entity.GetComponent<SpriteComponent>();
          storeSprite.OnSetEntity();
        }
      }

      if (entity.Name == "hole") {
        if (state.Status == CollisionStatus.INIT) {
          var beaverAgent = beaver.GetComponent<Agent>();
          var stateComponent = beaver.GetComponent<StateComponent<BeaverStates>>();
          beaverAgent.Remove<GoalToFollowPath>();
          stateComponent.CurrentState = BeaverStates.chop;
        }

        //spriteComponent.
        //Travel(rabbit, entity, state);

      }
    }

    /// <summary>
    /// Travel the specified rabbit, store and state.
    /// </summary>
    /// <returns>The travel.</returns>
    /// <param name="rabbit">Rabbit.</param>
    /// <param name="hole">Store.</param>
    /// <param name="state">State.</param>
    public static void Travel(Entity rabbit,
                              Entity hole,
                              CollisionState state)
    {
      var agent = rabbit.GetComponent<Agent>();

      var travel = hole.GetComponent<TravelComponent>();
      if (travel != null) {
        var dest = travel.Destinations.FirstOrDefault();
        if (dest != null) {
          var flightMap = Container.GetContainer().GetInstance<FlightMap>() as GameFlightMap;
          var hud = Container.GetContainer().GetInstance<Hud>();
          var otherHole = flightMap.GetEntityById(dest);
          if (otherHole != null) {
            var pos = otherHole.GetComponent<Agent>().Position;
            agent.Remove<GoalToFollowPath>();
            agent.Position = pos;
            var separate = new List<Entity>();
            separate.Add(hole);
            agent.Add(new GoalToSeparateFrom(x => separate), 36);

            hud.SetMessage($"Rabbit hole travel :) !");
          }
        }
      }
    }

    /// <summary>
    /// Gathers the food.
    /// </summary>
    /// <param name="rabbit">Rabbit.</param>
    /// <param name="store">Store.</param>
    /// <param name="state">State.</param>
    public static void GatherFood(Entity rabbit,
                                  Entity store,
                                  CollisionState state)
    {
      if (state.StopWatchValue > 1) {
        var foodUnits = 5;
        var health = store.GetComponent<HealthComponent>();
        var hud = Container.GetContainer().GetInstance<Hud>();
        var flightMap = Container.GetContainer().GetInstance<FlightMap>() as GameFlightMap;

        if (health.Damage < health.MaxHealth) {
          health.Damage += foodUnits;
          flightMap.AddToScore(rabbit.Id, foodUnits);
          hud.UpdateScore(flightMap.GetScore(rabbit.Id));
          state.ResetStopWatch();
          hud.SetMessage($"A fine day for picking carrots !");
        }
      }
    }
  }
}
