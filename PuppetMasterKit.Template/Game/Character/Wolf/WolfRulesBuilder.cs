using System;
using System.Diagnostics;
using System.Linq;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Rules;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Template.Game.Facts;

namespace PuppetMasterKit.Template.Game.Character.Wolf
{
  public class WolfRulesBuilder
  {
    private static int maxAttackRange = 200;
    private static int escapeRange = 20;

    /// <summary>
    /// Build the specified flightMap.
    /// </summary>
    /// <returns>The build.</returns>
    /// <param name="flightMap">Flight map.</param>
    public static RuleSystem<T> Build<T>(T flightMap) where T:  FlightMap
    {
      var ruleSystem = new RuleSystem<T>(flightMap);

      ruleSystem.Add(new Rule<T>(
        (entity, state, facts) => IsHungryAndCloseToTarget(entity, state, facts),
        (entity, state, facts) => ActionHungryAndCloseToTarget(entity, state, facts)
      ));

      ruleSystem.Add(new Rule<T>(
        (entity, state, facts) => IsHuntingAndPreyGotAway(entity, state, facts),
        (entity, state, facts) => ActionHuntingAndPreyGotAway(entity, state, facts)
      ));

      return ruleSystem;
    }

    /// <summary>
    /// Gets the closest target.
    /// </summary>
    /// <returns>The closest target.</returns>
    /// <param name="entity">Entity.</param>
    /// <param name="state">State.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    private static Entity GetClosestTarget<T>(Entity entity, T state) where T: FlightMap 
    {
      var thisAgent = entity.GetComponent<Agent>();
      var target = state
        .GetEntities(t => t.Name == "rabbit")
        .FirstOrDefault(x => {
          var agent = x.GetComponent<Agent>();
          var dist = Point.Distance(agent.Position, thisAgent.Position);
          return dist <= maxAttackRange;
        });
      return target;
    }

    /// <summary>
    /// Ises the hungry and close to target.
    /// </summary>
    /// <returns><c>true</c>, if hungry and close to target was ised, <c>false</c> otherwise.</returns>
    /// <param name="entity">Entity.</param>
    /// <param name="state">State.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    private static bool IsHungryAndCloseToTarget<T>(Entity entity, T state, FactSet facts) where T : FlightMap
    {
      return GetClosestTarget(entity, state)!=null;
    }

    /// <summary>
    /// Ises the hunting and prey got away.
    /// </summary>
    /// <returns><c>true</c>, if hunting and prey got away was ised, <c>false</c> otherwise.</returns>
    /// <param name="entity">Entity.</param>
    /// <param name="state">State.</param>
    /// <param name="facts">Facts.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    private static bool IsHuntingAndPreyGotAway<T>(Entity entity, T state, FactSet facts) where T : FlightMap
    {
      var thisAgent = entity.GetComponent<Agent>();
      var hunt = facts.GetFact<Hunt>();
      if(hunt != null){
        var target = hunt.GetTarget().GetComponent<Agent>();
        var dist = Point.Distance(target.Position, thisAgent.Position);
        return dist > maxAttackRange + escapeRange;
      }
      return false;
    }

    /// <summary>
    /// Actions the hungry and close to target.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="state">State.</param>
    /// <param name="facts">Facts.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    private static void ActionHungryAndCloseToTarget<T>(Entity entity, T state, FactSet facts) where T : FlightMap
    {
      var target = GetClosestTarget(entity, state);
      var thisAgent = entity.GetComponent<Agent>();
      var hunt = facts.GetFact<Hunt>();
      if (target != null) {
        if (hunt == null || hunt.GetTarget() != target) {
          facts.Assert(new Hunt(target));
          facts.Retract<Idle>();
        }
      }
    }

    /// <summary>
    /// Actions the hunting and prey got away.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="state">State.</param>
    /// <param name="facts">Facts.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    private static void ActionHuntingAndPreyGotAway<T>(Entity entity, T state, FactSet facts) where T : FlightMap
    {
      var hunt = facts.GetFact<Hunt>();
      if (hunt != null) {
        facts.Retract<Hunt>();
        facts.Assert(new Idle());
      }
    }
  }
}
