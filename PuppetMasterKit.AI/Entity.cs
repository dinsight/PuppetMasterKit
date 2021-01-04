using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Utility.Extensions;
using System.Diagnostics;

namespace PuppetMasterKit.AI
{
  public class Entity : IDisposable
  {
    private List<Component> components;

    private bool disposedValue;

    public String Id { get; set; }

    public EntityBucketId BucketId { get; set; }

    public String Name { get; set; }

    /// <summary>
    /// Initialization
    /// </summary>
    private Entity()
    {
      Id = Guid.NewGuid().ToString();
      components = new List<Component>();
    }

    /// <summary>
    /// Add the specified component.
    /// </summary>
    /// <returns>The add.</returns>
    /// <param name="component">Component.</param>
    public Entity Add(Component component)
    {
      var type = component.GetType();
      if(!components.Exists(x => x.GetType() == type)){
        components.Add(component);
      }
      return this;
    }

    /// <summary>
    /// Remove this instance.
    /// </summary>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public void Remove<T>()
    {
      var toRemove = components.Where(x => x is T).ToList();
      foreach (var item in toRemove) {
        item.System = null;
        components.Remove(item);
      }
    }

    /// <summary>
    /// Gets the component.
    /// </summary>
    /// <returns>The component.</returns>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public T GetComponent<T>() where T : Component
    {
      return components.FirstOrDefault(x => x is T) as T;
    }

    /// <summary>
    /// Gets the components.
    /// </summary>
    /// <returns>The components.</returns>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public IEnumerable<T> GetComponents<T>() where T : class
    {
      return components?.Where(x => x is T).Select(z => z as T);
    }

    #region Cleanup
    
    /// <summary>
    /// Cleanup this instance.
    /// </summary>
    private void Cleanup()
    {
      if (components != null) {
        components.ForEach(x => x.Cleanup());
        components.Clear();
        components = null;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue) {
        if (disposing) {
          Cleanup();
        }
        disposedValue = true;
      }
    }

    public void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }
    #endregion

    #region Builder
    public class EntityBuilder
    {
      private Entity entity;

      /// <summary>
      /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.EntityBuilder"/> class.
      /// </summary>
      private EntityBuilder()
      {
        entity = new Entity();
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.EntityBuilder"/> class.
      /// </summary>
      /// <param name="entity">Entity.</param>
      private EntityBuilder(Entity entity)
      {
        this.entity = entity;
      }

      /// <summary>
      /// Build the specified entity.
      /// </summary>
      /// <returns>The build.</returns>
      /// <param name="entity">Entity.</param>
      public static EntityBuilder Builder(Entity entity = null)
      {
        if (entity != null) {
          return new EntityBuilder(entity);
        }
        return new EntityBuilder();
      }

      /// <summary>
      /// Gets the entity.
      /// </summary>
      /// <returns>The entity.</returns>
      public Entity Build()
      {
        entity.components.ForEach(x => x.SetEntity(entity));
        return entity;
      }

      /// <summary>
      /// Register entity's components having a cetrain type with a component system
      /// </summary>
      /// <returns>The entity.</returns>
      /// <param name="withSystem">With system.</param>
      /// <typeparam name="T">The 1st type parameter.</typeparam>
      public EntityBuilder Register<T>(ComponentSystem withSystem)
      {
        var toRegister = entity.components.Where(x => x is T);
        withSystem.AddRange(toRegister.ToArray());
        return this;
      }

      /// <summary>
      /// Register all the entity's components with a component system
      /// </summary>
      /// <returns>The register.</returns>
      /// <param name="withSystem">With system.</param>
      public EntityBuilder Register(ComponentSystem withSystem)
      {
        withSystem.AddRange(entity.components.ToArray());
        return this;
      }

      /// <summary>
      /// Registers the entity with the system and also adds compoments to the entity
      /// </summary>
      /// <returns>The with.</returns>
      /// <param name="withSystem">With system.</param>
      /// <param name="components">Components.</param>
      public EntityBuilder With(ComponentSystem withSystem,
                 params Component[] components)
      {
        components.ForEach(x => {
          entity.Add(x);
          withSystem.Add(x);
        });
        return this;
      }

      /// <summary>
      /// Withs the name.
      /// </summary>
      /// <returns>The name.</returns>
      /// <param name="name">Name.</param>
      public EntityBuilder WithName(String name)
      {
        this.entity.Name = name;
        return this;
      }
    }
    #endregion
  }
}
