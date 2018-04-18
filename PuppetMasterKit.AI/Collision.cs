using System;
namespace PuppetMasterKit.AI
{
  internal class Collision
  {
    public WeakReference<Entity> WithEntity { get; set; }

    public CollisionState State { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.CollisionNotificationEntry"/> class.
    /// </summary>
    /// <param name="entity">Entity.</param>
    public Collision(Entity entity)
    {
      WithEntity = new WeakReference<Entity>(entity);
      State = new CollisionState();
    }
  }
}
