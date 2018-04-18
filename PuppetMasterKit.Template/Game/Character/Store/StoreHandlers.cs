using System;
using System.Diagnostics;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Goals;
using PuppetMasterKit.AI.Rules;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Template.Game.Facts;

namespace PuppetMasterKit.Template.Game.Character.Rabbit
{
  public class StoreHandlers : FactHandler
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
    public static void OnMoveToPoint(Entity entity, Point location)
    {
      
    }
  }
}
