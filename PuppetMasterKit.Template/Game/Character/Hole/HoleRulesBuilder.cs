using System;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Rules;
using PuppetMasterKit.Template.Game.Facts;

namespace PuppetMasterKit.Template.Game.Character.Rabbit
{
  public class HoleRulesBuilder
  {
    /// <summary>
    /// Build the specified flightMap.
    /// </summary>
    /// <returns>The build.</returns>
    /// <param name="flightMap">Flight map.</param>
    public static RuleSystem<T> Build<T>(T flightMap) where T:  FlightMap
    {
      var ruleSystem = new RuleSystem<T>(flightMap);

      return ruleSystem;
    }
  }
}
