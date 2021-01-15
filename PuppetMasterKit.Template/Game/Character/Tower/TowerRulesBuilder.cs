using System;
using System.Linq;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Rules;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Template.Game.Facts;

namespace PuppetMasterKit.Template.Game.Character.Tower
{
  public class TowerRulesBuilder
  {
    private static int maxAttackRange = 1200;

    public static RuleSystem<T> Build<T>(T flightMap) where T : FlightMap
    {
      var ruleSystem = new RuleSystem<T>(flightMap);

      ruleSystem.Add(new Rule<T>(
        (entity, state, facts) => IsTargetInRange(entity, state, facts),
        (entity, state, facts) => AttackTraget(entity, state, facts),
        (entity, state, facts) => facts.Retract<Attack>()
      ));

      return ruleSystem;
    }

    private static void AttackTraget<T>(Entity entity, T state, FactSet facts) where T : FlightMap
    {
      var target = GetClosestTarget(entity, state);
      var hunt = facts.GetFact<Attack>();
      if (target != null) {
        if (hunt == null || hunt.GetTarget() != target) {
          facts.Assert(new Attack(target));
          facts.Retract<Idle>();
        }
      }
    }

    private static bool IsTargetInRange<T>(Entity entity, T state, FactSet facts) where T : FlightMap
    {
      return GetClosestTarget(entity, state) != null;
    }

    private static Entity GetClosestTarget<T>(Entity entity, T state) where T : FlightMap
    {
      var thisAgent = entity.GetComponent<Agent>();
      var target = state
        .GetEntities(t => t.Name == "wolf")
        .FirstOrDefault(x => {
          var agent = x.GetComponent<Agent>();
          var dist = Point.Distance(agent.Position, thisAgent.Position);
          return dist <= maxAttackRange;
        });
      return target;
    }

  }
}
