using System;
using LightInject;
using System.Linq;
using System.Diagnostics;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.AI.Rules;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Template.Game.Facts;
using PuppetMasterKit.Template.Game.Components;

namespace PuppetMasterKit.Template.Game.Character.Rabbit
{
  public class RabbitHandlers : FactHandler
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
    /// Ons the scene touched.
    /// </summary>
    /// <param name="location">Location.</param>
    public static void OnMoveToPoint_test(Entity entity, Point location)
    {
      var agent = entity.GetComponent<Agent>();
      if (agent == null)
        return;

      location = new Point(150, 320);
      //remove existing follow path command
      agent.Remove<GoalToFollowPath>();
      //create new goal. Makes sure the goal is deleted upon arrival
      var goToPoint = new GoalToFollowPath(new Point[] { agent.Position, location })
        .WhenArrived((x, p) => { });
      agent.Add(goToPoint, 3);
    }

    public static void OnMoveToPoint(Entity entity, Point location)
    {
      var agent = entity.GetComponent<Agent>();
      if (agent == null)
        return;

      //remove existing follow path command
      agent.Remove<GoalToFollowPath>();
      //create new goal. Makes sure the goal is deleted upon arrival
      var goToPoint = new GoalToFollowPath(new Point[] { agent.Position, location })
        .WhenArrived((x, p) => { });
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
    /// <param name="rabbit">Rabbit.</param>
    /// <param name="entity">Entity.</param>
    /// <param name="state">State.</param>
    public static void HandleCollision(Entity rabbit,
                              Entity entity,
                              CollisionState state)
    {
      if(entity.Name == "store"){
        GatherFood(rabbit, entity, state);
      }

      if(entity.Name == "hole"){
        Travel(rabbit, entity, state);
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
      if(travel!=null){
        var dest = travel.Destinations.FirstOrDefault();
        if(dest!=null){
          var flightMap = Container.GetContainer().GetInstance<FlightMap>() as GameFlightMap;
          var hud = Container.GetContainer().GetInstance<Hud>();
          var otherHole = flightMap.GetEntityById(dest);
          if(otherHole!=null){
            var pos = otherHole.GetComponent<Agent>().Position;
            agent.Remove<GoalToFollowPath>();
            agent.Position = pos + new Point(36, 36);
            //agent.Stop();
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

        if(health.Damage < health.MaxHealth){
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
