﻿using System;
using LightInject;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.AI.Rules;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Template.Game.Facts;
using PuppetMasterKit.Utility.Configuration;

namespace PuppetMasterKit.Template.Game.Character.Tower
{
  public class TowerHandlers : FactHandler
  {
    public TowerHandlers()
    {
    }

    internal static void OnTouched(Entity obj)
    {
      
    }

    public void Handle(Entity entity, Attack fact)
    {
      var targetEntity = fact.GetTarget();
      var state = entity.GetComponent<StateComponent<TowerStates>>();
      if (state.CurrentState == TowerStates.ready) {
        state.CurrentState = TowerStates.attack;
      }

      if (state.CurrentState == TowerStates.attack) {
        if (targetEntity != null) {
          var target = targetEntity?.GetComponent<Agent>();
          OnAttackPoint(entity, target?.Position);
        }
      }
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
