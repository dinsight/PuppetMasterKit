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
          new TouchComponent(OnTargetTouched, OnSceneTouched),
          new Agent())
        .WithName("rabbit")
        .GetEntity();
      
      return entity;
    }

    /// <summary>
    /// Ons the target touched.
    /// </summary>
    /// <param name="entity">Entity.</param>
    private static void OnTargetTouched(Entity entity)
    {
      
    }

    /// <summary>
    /// Ons the scene touched.
    /// </summary>
    /// <param name="location">Location.</param>
    private static void OnSceneTouched(Point location)
    {

    }
  }
}
