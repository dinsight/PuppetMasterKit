using System;
using PuppetMasterKit.AI.Components;
using NUnit.Framework;
using PuppetMasterKit.AI;
using PuppetMasterKit.Template.Test.Bindings;
using PuppetMasterKit.Utility.Attributes;

namespace PuppetMasterKit.UnitTest
{
    [TestFixture]
    public class ComponentSystemTest
    {
        [Test]
        public void TestRemove()
        {
            var system = new ComponentSystem();
            system.Add(new HealthComponent(1));
            system.Add(new HealthComponent(1));
            system.Add(new FoodComponent());
            system.Add(new PhysicsComponent());

            //system.Remove<HealthComponent>();

            Assert.True(system.Count == 2);
        }

        [Test]
        public void TestAgentUpdate()
        {
            Registration.RegisterBindings();
            var agent = new Agent();
            var entity = new Entity()
				.Add(new SpriteComponent("test"))
				.Add(new SpriteComponent("none"))
                .Add(agent);

            agent.Update(0);
        }

		enum TestEnum {
			[StringValue("xxxx")] idle
		}
		[Test]
		public void TestState()
		{
			var state = new StateComponent<TestEnum>(TestEnum.idle);
			StateComponent comp = state;
			Assert.AreEqual("xxxx", state.ToString(), "Unexpected enum string value");
		}
    }
}
