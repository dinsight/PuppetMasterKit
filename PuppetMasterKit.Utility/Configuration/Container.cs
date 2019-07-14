using System;
using LightInject;

namespace PuppetMasterKit.Utility.Configuration
{
    public static class Container
    {
        static ServiceContainer container = new ServiceContainer();

        /// <summary>
        /// Initializes the <see cref="T:PuppetMasterKit.Configuration.Container"/> class.
        /// </summary>
        static Container()
        {
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <returns>The container.</returns>
        public static ServiceContainer GetContainer(){
            return container;
        }
    }
}
