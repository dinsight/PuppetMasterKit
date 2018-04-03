using System;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.Template.Game.Character
{
  public class RabbitBuilder
  {
    /// <summary>
    /// Build the specified componentSystem.
    /// </summary>
    /// <returns>The build.</returns>
    /// <param name="componentSystem">Component system.</param>
    public static Entity Build(ComponentSystem componentSystem)
    {
      var entity = EntityBuilder.Build()
        .With(componentSystem,
          new StateComponent<RabbitStates>(RabbitStates.idle),
          new SpriteComponent("rabbit", new Size(30, 30)),
          new Agent())
        .GetEntity();
      
      return entity;
    }
  }
}
