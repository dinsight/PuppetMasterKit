using System;
using PuppetMasterKit.AI.Components;
using NUnit.Framework;
using PuppetMasterKit.AI;
using PuppetMasterKit.Template.Test.Bindings;

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

        [Test]
        public void TestAgentUpdate()
        {
            Registration.RegisterBindings();
            var agent = new Agent();
            var entity = new Entity()
                .Add(new SpriteComponent<string>("test", string.Empty))
                .Add(new SpriteComponent<string>("none", string.Empty))
                .Add(agent);

            agent.Update(0);
        }
    }
}
