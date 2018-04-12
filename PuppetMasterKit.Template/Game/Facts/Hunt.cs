using System;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Rules;

namespace PuppetMasterKit.Template.Game.Facts
{
  public class Hunt : Fact
  {
    WeakReference<Entity> target;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Template.Game.Facts.Hunt"/> class.
    /// </summary>
    /// <param name="target">Target.</param>
    public Hunt(Entity target)
    {
      this.target = new WeakReference<Entity>(target);
    }

    /// <summary>
    /// Gets the target.
    /// </summary>
    /// <returns>The target.</returns>
    public Entity GetTarget(){
      Entity result = null;
      target.TryGetTarget(out result);
      return result;
    }
  }
}
