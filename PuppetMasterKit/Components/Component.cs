using System;
namespace PuppetMasterKit.Components
{
    public class Component
    {
        public ComponentSystem System { get; set; }

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
        public void update(double deltaTime)
        {
            
        }
    }
}
