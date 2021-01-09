using System;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Rules;

namespace PuppetMasterKit.Template.Game.Character.Tower
{
  public class TowerRulesBuilder
  {
    public static RuleSystem<T> Build<T>(T flightMap) where T : FlightMap
    {
      var ruleSystem = new RuleSystem<T>(flightMap);

      return ruleSystem;
    }
  }
}
