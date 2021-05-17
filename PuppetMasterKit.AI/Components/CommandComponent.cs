using System;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI.Components
{
  public class CommandComponent : Component
  {
    public Action<Entity> OnEntityTouched { get; set; }

    public Action<Entity,Point> OnLocationTouched { get; set; }

    public Action<Entity, Point> OnAttackPoint { get; set; }

    //public Action<Entity> OnMoveToEntity { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Components.TouchComponent"/> class.
    /// </summary>
    /// <param name="onTouched">Target touched.</param>
    /// <param name="onMoveToPoint">Scene touched.</param>
    public CommandComponent()
    {
    }
  }
}
