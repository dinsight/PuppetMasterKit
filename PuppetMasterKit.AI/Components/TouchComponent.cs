using System;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI.Components
{
  public class TouchComponent : Component
  {
    public Action<Entity> OnTargetTouched { get; private set; }

    public Action<Point> OnSceneTouched { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Components.TouchComponent"/> class.
    /// </summary>
    /// <param name="targetTouched">Target touched.</param>
    /// <param name="sceneTouched">Scene touched.</param>
    public TouchComponent(Action<Entity> targetTouched, Action<Point> sceneTouched )
    {
      OnTargetTouched = targetTouched;
      OnSceneTouched = sceneTouched;
    }

  }
}
