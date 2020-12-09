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
    /// Gets or sets the components.
    /// </summary>
    /// <value>The components.</value>
    public List<Component> Components { 
      get { return components; } 
      set { 
        components = value;
        components.ForEach(c => c.SetEntity(this));
      } 
    }

    /// <summary>
    /// Initialization
    /// </summary>
    public Entity()
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
      component.SetEntity(this);
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
      return components.Where(x => x is T).Select(z => z as T);
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
  }
}
