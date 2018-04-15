using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.Utility;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI.Components
{
  public class CollisionComponent : Component
  {

    private float collisionRange;

    private Func<IEnumerable<Entity>> entitiesProvider;

    private Action<Entity, Entity> handler;
    private IEnumerable<Entity> enumerable;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Components.CollisionComponent"/> class.
    /// </summary>
    /// <param name="entitiesProvider">Entities provider.</param>
    /// <param name="handler">Handler.</param>
    /// <param name="collisionRange">Collision range.</param>
    public CollisionComponent(Func<IEnumerable<Entity>> entitiesProvider, 
                              Action<Entity, Entity> handler,
                              float collisionRange = 10)
    {
      this.entitiesProvider = entitiesProvider;
      this.collisionRange = collisionRange;
      this.handler = handler;
    }

    /// <summary>
    /// Detect this instance.
    /// </summary>
    private void Detect()
    {
      var inRange = entitiesProvider().Where(x => {
        var thisAgent = Entity.GetComponent<Agent>();
        var agent = x.GetComponent<Agent>();
        return Point.Distance(thisAgent.Position, agent.Position) <= collisionRange;
      });

      inRange.ForEach(x => handler(Entity, x));
    }

    /// <summary>
    /// Update the specified deltaTime.
    /// </summary>
    /// <returns>The update.</returns>
    /// <param name="deltaTime">Delta time.</param>
    public override void Update(double deltaTime)
    {
      Detect();
      base.Update(deltaTime);
    }
  }
}
