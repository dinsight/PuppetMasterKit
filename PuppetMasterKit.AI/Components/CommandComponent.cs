using System;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI.Components
{
  public class CommandComponent : Component
  {
    public Action<Entity> OnTouched { get; private set; }

    public Action<Entity,Point> OnUserTouchedPoint { get; private set; }

    public Action<Entity> OnMoveToEntity { get; private set; }

    public Action<Entity, Point> OnBuild { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Components.TouchComponent"/> class.
    /// </summary>
    /// <param name="onTouched">Target touched.</param>
    /// <param name="onMoveToPoint">Scene touched.</param>
    public CommandComponent(Action<Entity> onTouched, 
                            Action<Entity,Point> onUserTouchedPoint,
                            Action<Entity, Point> onBuild)
    {
      OnTouched = onTouched;
      OnUserTouchedPoint = onUserTouchedPoint;
      OnBuild = onBuild;
    }
  }
}
