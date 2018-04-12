using System;
using System.Linq;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Rules;
using PuppetMasterKit.Template.Game.Facts;

namespace PuppetMasterKit.Template.Game.Character.Wolf
{
  public class WolfRulesBuilder
  {
    /// <summary>
    /// Build the specified flightMap.
    /// </summary>
    /// <returns>The build.</returns>
    /// <param name="flightMap">Flight map.</param>
    public static RuleSystem<T> Build<T>(T flightMap) where T:  FlightMap
    {
      var ruleSystem = new RuleSystem<T>(flightMap);
      ruleSystem.Add(new Rule<T>(x => true, (p,s) => {
        var target = flightMap.GetEntities(t => t.Name == "rabbit").FirstOrDefault();
        s.Assert(new Hunt(target));
      }));
      return ruleSystem;
    }
  }
}
