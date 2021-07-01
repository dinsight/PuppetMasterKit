using System;
using System.Linq;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Rules;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Template.Game.Facts;

namespace PuppetMasterKit.Template.Game.Character.Fishery
{
  public class FisheryRulesBuilder
  {
    private static int maxAttackRange = 1200;

    public static RuleSystem<T> Build<T>(T flightMap) where T : FlightMap
    {
      var ruleSystem = new RuleSystem<T>(flightMap);

      return ruleSystem;
    }


  }
}
