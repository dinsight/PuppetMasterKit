using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.Utility.Extensions;
using System.Collections.Concurrent;

namespace PuppetMasterKit.AI.Components
{
  public class ComponentSystem
  {
    private List<Component> components = new List<Component>();

    /// <summary>
    /// Gets the component count
    /// </summary>
    /// <value>The count.</value>
    public int Count {
      get { return components.Count; }
    }

    /// <summary>
    /// Initialization
    /// </summary>
    public ComponentSystem()
    {
    }

    /// <summary>
    /// Periodic update
    /// </summary>
    /// <returns>The update.</returns>
    /// <param name="deltaTime">The time interval between subsequent update calls</param>
    public void Update(double deltaTime)
    {
      foreach (var component in components) {
        component.Update(deltaTime);
      }
    }

    /// <summary>
    /// Add the specified instance.
    /// </summary>
    /// <returns>The add.</returns>
    /// <param name="instance">Instance.</param>
    public void Add(Component instance)
    {
      instance.System = this;
      components.Add(instance);
    }

    /// <summary>
    /// Adds a list of components
    /// </summary>
    /// <param name="instances">Instances.</param>
    public void AddRange(Component[] instances)
    {
      instances.ForEach(x => Add(x));
    }

    /// <summary>
    /// 
    /// </summary>
    public void CleanupOrphanComponents() {
      if (components.Where(x => x.System == null).Count() > 0) {
        var toRemove = components.Where(x => x.System == null).ToList();
        toRemove.ForEach(component =>
          components.Remove(component)
        );
      }
    }
  }
}
