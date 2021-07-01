using System;
using LightInject;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.AI.Rules;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Template.Game.Facts;
using PuppetMasterKit.Utility.Configuration;

namespace PuppetMasterKit.Template.Game.Character.Fishery
{
  public class FisheryHandlers : FactHandler
  {
    public FisheryHandlers()
    {
    }

    internal static void OnTouched(Entity obj)
    {
      
    }

    public void Handle(Entity entity, Attack fact)
    {
      var targetEntity = fact.GetTarget();
      var state = entity.GetComponent<StateComponent<FisheryStates>>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="location"></param>
    public static void OnAttackPoint(Entity entity, Point location)
    {
      if (location != null) {
        var weapon = entity.GetComponent<RangeWeaponComponent>();
        weapon.Fire(location);
      }
    }
  }
}
