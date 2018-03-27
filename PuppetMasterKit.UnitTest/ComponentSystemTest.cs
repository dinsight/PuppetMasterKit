using System;
using PuppetMasterKit.Components;
using NUnit.Framework;

namespace PuppetMasterKit.UnitTest
{
    [TestFixture]
    public class ComponentSystemTest
    {
        [Test]
        public void TestRemove()
        {
            var system = new ComponentSystem();
            system.Add(new HealthComponent());
            system.Add(new HealthComponent());
            system.Add(new FoodComponent());
            system.Add(new PhysicsComponent());

            system.Remove<HealthComponent>();

            Assert.True(system.Count == 2);
        }
    }
}
