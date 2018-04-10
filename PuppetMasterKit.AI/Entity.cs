using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.AI
{
  public class Entity : IDisposable
  {
    public List<Component> Components { get; private set; }

    public String Id { get; private set; }

    public EntityBucketId BucketId { get; set; }

    public String Name { get; set; }

    /// <summary>
    /// Initialization
    /// </summary>
    public Entity()
    {
      Id = Guid.NewGuid().ToString();
      Components = new List<Component>();
    }

    /// <summary>
    /// Add the specified component.
    /// </summary>
    /// <returns>The add.</returns>
    /// <param name="component">Component.</param>
    public Entity Add(Component component)
    {
      component.SetEntity(this);
      Components.Add(component);
      return this;
    }

    /// <summary>
    /// Remove this instance.
    /// </summary>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public void Remove<T>()
    {
      var toRemove = Components.Where(x => x is T).ToList();
      foreach (var item in toRemove) {
        item.System = null;
        Components.Remove(item);
      }
    }

    /// <summary>
    /// Gets the component.
    /// </summary>
    /// <returns>The component.</returns>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public T GetComponent<T>() where T : Component
    {
      return Components.FirstOrDefault(x => x is T) as T;
    }

    /// <summary>
    /// Gets the components.
    /// </summary>
    /// <returns>The components.</returns>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public IEnumerable<T> GetComponents<T>() where T : class
    {
      return Components.Where(x => x is T).Select(z => z as T);
    }

    /// <summary>
    /// Cleanup this instance.
    /// </summary>
    public void Cleanup()
    {
      Components.ForEach(x => x.Cleanup());
      Components.Clear();
    }

    /// <summary>
    /// IDisposable
    /// </summary>
    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    /// <summary>
    /// Dispose instance.
    /// </summary>
    /// <returns>The dispose.</returns>
    /// <param name="disposing">If set to <c>true</c> disposing.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue) {
        if (disposing) {
          Cleanup();
        }

        disposedValue = true;
      }
    }

    /// <summary>
    /// Releases all resource used by the <see cref="T:PuppetMasterKit.Entity"/> object.
    /// </summary>
    /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="T:PuppetMasterKit.Entity"/>. The
    /// <see cref="Dispose"/> method leaves the <see cref="T:PuppetMasterKit.Entity"/> in an unusable state. After
    /// calling <see cref="Dispose"/>, you must release all references to the <see cref="T:PuppetMasterKit.Entity"/>
    /// so the garbage collector can reclaim the memory that the <see cref="T:PuppetMasterKit.Entity"/> was occupying.</remarks>
    public void Dispose()
    {
      Dispose(true);
    }
    #endregion
  }
}
