using System;
namespace PuppetMasterKit.AI.Components
{
  public class Component
  {
    public ComponentSystem System { get; set; }

    public Entity Entity { get; private set; }

    /// <summary>
    /// Initializtion
    /// </summary>
    public Component()
    {
    }

    /// <summary>
    /// Periodic update
    /// </summary>
    /// <returns>The update.</returns>
    /// <param name="deltaTime">The time interval between subsequent update calls</param>
    public virtual void Update(double deltaTime)
    {

    }
    /// <summary>
    /// Ons the set entity.
    /// </summary>
    public virtual void OnSetEntity(){
      
    }

    /// <summary>
    /// Sets the entity.
    /// </summary>
    /// <returns>The entity.</returns>
    /// <param name="entity">Entity.</param>
    public Component SetEntity(Entity entity)
    {
      this.Entity = entity;
      OnSetEntity();
      return this;
    }

    /// <summary>
    /// Cleanup resources
    /// </summary>
    public virtual void Cleanup()
    {
      Entity = null;
      System = null;
    }
  }
}
