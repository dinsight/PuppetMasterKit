using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.Components;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit
{
    public class Entity : IDisposable
    {
        public List<Component> Components { get; private set; }

        /// <summary>
        /// Initialization
        /// </summary>
        public Entity()
        {
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
            foreach (var item in toRemove)
            {
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
        /// Register entity's components having a cetrain type with a component system
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="withSystem">With system.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public Entity Register<T>(ComponentSystem withSystem)
        {
            var toRegister = Components.Where(x => x is T);
            withSystem.AddRange(toRegister.ToArray());
            return this;
        }

        /// <summary>
        /// Register all the entity's components with a component system
        /// </summary>
        /// <returns>The register.</returns>
        /// <param name="withSystem">With system.</param>
        public Entity Register(ComponentSystem withSystem)
        {
            withSystem.AddRange(Components.ToArray());
            return this;
        }

        /// <summary>
        /// Registers the entity with the system and also adds compoments to the entity
        /// </summary>
        /// <returns>The with.</returns>
        /// <param name="withSystem">With system.</param>
        /// <param name="components">Components.</param>
        public Entity With(ComponentSystem withSystem, 
                           params Component[] components)
        {
            components.ForEach(x=>{
                this.Add(x);
                withSystem.Add(x);
            });
            return this;
        }

        /// <summary>
        /// Cleanup this instance.
        /// </summary>
        public void Cleanup()
        {
            Components.ForEach(x=>x.Cleanup());
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
            if (!disposedValue)
            {
                if (disposing)
                {
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
