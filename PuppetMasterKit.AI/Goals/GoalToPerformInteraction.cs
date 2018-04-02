using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI.Goals
{
  public class GoalToPerformInteraction : Goal
  {
    private Func<Entity, ICollection<Entity>> entitiesProvider;

    private Func<Entity, Entity, bool> handler;

    private float interactionRadius;

    private int currentIndex;

    private int direction;

    private Activity activity = new Activity();

    /// <summary>
    /// Activity.
    /// </summary>
    private class Activity
    {
      public bool IsInProgress { get; set; }
      public Entity ThisEntiy { get; set; }
      public Entity WithEntity { get; set; }
      public Activity()
      {
        IsInProgress = false;
        ThisEntiy = null;
        WithEntity = null;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Goals.GoalToPerformInteraction"/> class.
    /// </summary>
    /// <param name="entitiesProvider">Entities to interrace with.</param>
    /// <param name="interactionRadius">interactionEntities: entities for 
    /// which activities will be performed</param>
    public GoalToPerformInteraction(Func<Entity, ICollection<Entity>> entitiesProvider,
                                        float interactionRadius = 10)
    {
      this.entitiesProvider = entitiesProvider;
      this.interactionRadius = interactionRadius;
      this.currentIndex = 0;
      this.direction = -1;
    }

    /// <summary>
    /// Set interaction handler
    /// </summary>
    /// <returns>The handler.</returns>
    /// <param name="handler">Handler.</param>
    public GoalToPerformInteraction WithHandler(Func<Entity, Entity, bool> handler)
    {
      this.handler = handler;
      return this;
    }

    /// <summary>
    /// Force the specified agent.
    /// </summary>
    /// <returns>The force.</returns>
    /// <param name="agent">Agent.</param>
    public override Vector Force(Agent agent)
    {
      var entities = entitiesProvider(agent.Entity).ToList();
      if (entitiesProvider(agent.Entity).Count == 0 || agent.Position == null)
        return Vector.Zero;

      if (CheckOngoingActivity())
        return Vector.Zero;

      var currentEntity = entities[currentIndex];
      var otherAgent = currentEntity.GetComponent<Agent>();
      var target = otherAgent.Position;

      if (target != null) {
        var velocity = (agent.Velocity + Steering(agent, target));
        //When we get close enough to this point, vector in on the next one
        var dist = velocity + agent.Position - target;

        if (dist.Magnitude() <= interactionRadius) {
          if (currentIndex == entities.Count - 1 ||
              currentIndex == 0) {
            direction *= -1;
          }
          currentIndex += direction;
          StartActivity(agent.Entity, currentEntity);
        } else {
          ClearActivity();
        }

        return velocity;
      }

      return Vector.Zero;
    }

    /// <summary>
    /// Check if there is an ongoing activity
    /// </summary>
    /// <returns><c>true</c>, if ongoing activity was checked, <c>false</c> otherwise.</returns>
    private bool CheckOngoingActivity()
    {
      if (activity.IsInProgress && handler != null) {
        return activity.IsInProgress =
            handler(activity.ThisEntiy, activity.WithEntity);
      }

      return false;
    }

    /// <summary>
    /// Starts an activity between two entities
    /// </summary>
    /// <param name="thisEntity">This entity.</param>
    /// <param name="withEntity">With entity.</param>
    private void StartActivity(Entity thisEntity, Entity withEntity)
    {
      if (handler != null) {
        activity.IsInProgress = handler(thisEntity, withEntity);
        activity.ThisEntiy = thisEntity;
        activity.WithEntity = withEntity;
      }
    }

    /// <summary>
    /// Clears the in progress activity
    /// </summary>
    private void ClearActivity()
    {
      activity.IsInProgress = false;
      activity.ThisEntiy = null;
      activity.WithEntity = null;
    }
  }
}
