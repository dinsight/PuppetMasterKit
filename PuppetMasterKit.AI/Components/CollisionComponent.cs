using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.Utility.Extensions;
using PuppetMasterKit.Graphics.Geometry;
using System.Diagnostics;

namespace PuppetMasterKit.AI.Components
{
  public partial class CollisionComponent : Component
  {
    private float collisionRange;

    private Func<Entity, IEnumerable<Entity>> entitiesProvider;

    private Action<Entity, Entity, CollisionState> handler;

    private Dictionary<string, Collision> notifiedEntities;


    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Components.CollisionComponent"/> class.
    /// </summary>
    /// <param name="entitiesProvider">Entities provider.</param>
    /// <param name="handler">Handler.</param>
    /// <param name="collisionRange">Collision range.</param>
    public CollisionComponent(Func<Entity, IEnumerable<Entity>> entitiesProvider, 
                              Action<Entity, Entity, CollisionState> handler,
                              float collisionRange = 10)
    {
      this.entitiesProvider = entitiesProvider;
      this.collisionRange = collisionRange;
      this.handler = handler;
      this.notifiedEntities = new Dictionary<string, Collision>();
    }

    /// <summary>
    /// Detect this instance.
    /// </summary>
    private void Detect(double deltaTime)
    {
      var thisAgent = Entity.GetComponent<Agent>();

      var inRange = entitiesProvider(Entity).Where(x => {
        var agent = x.GetComponent<Agent>();
        return Point.Distance(thisAgent.Position, agent.Position) <= collisionRange;
      });

      var ids = inRange.Select(x => x.Id);
      var newCollisions = inRange.Where(x => !notifiedEntities.ContainsKey(x.Id));

      var doneCollisions = notifiedEntities
        .Where(x=>!ids.Contains(x.Key))
        .Select(a=>a.Value)
        .ToArray();
      var inProgress = notifiedEntities
        .Where(x => ids.Contains(x.Key))
        .Select(a => a.Value)
        .ToArray();

      newCollisions.ForEach(x => {
        var entry = new Collision(x);
        notifiedEntities.Add(x.Id, entry);
        handler(Entity, x, entry.State); 
      });
     
      doneCollisions.ForEach(x => {
        Entity temp;
        if (x.WithEntity.TryGetTarget(out temp)) {
          x.State.Status = CollisionStatus.DONE;
          handler(Entity, temp, x.State);
          notifiedEntities.Remove(temp.Id);
        }});

      inProgress.ForEach(x => {
        Entity temp;
        if (x.WithEntity.TryGetTarget(out temp)) {
          x.State.Status = CollisionStatus.IN_PROGRESS;
          x.State.ElapsedTime += deltaTime;
          x.State.StopWatchValue += deltaTime;
          handler(Entity, temp, x.State);
        }});
    }

    /// <summary>
    /// Update the specified deltaTime.
    /// </summary>
    /// <returns>The update.</returns>
    /// <param name="deltaTime">Delta time.</param>
    public override void Update(double deltaTime)
    {
      Detect(deltaTime);
      base.Update(deltaTime);
    }
  }
}
