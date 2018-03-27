﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace PuppetMasterKit.Components
{
    public class ComponentSystem
    {
        private List<Component> components = new List<Component>();

        /// <summary>
        /// Gets the component count
        /// </summary>
        /// <value>The count.</value>
        public int Count 
        {
            get { return components.Count;  }
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
        public void update(double deltaTime) 
        {
            foreach (var component in components)
            {
                component.update(deltaTime);
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
        /// Remove a component.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        public void Remove<T>()
        {
            var toRemove = components.Where(x => x is T).ToList();
            foreach (var item in toRemove)
            {
                item.System = null;
                components.Remove(item);
            }
        }
    }
}
