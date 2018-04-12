
using System;
using NUnit.Framework;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Rules;

namespace PuppetMasterKit.UnitTest.UnitTests
{
  [TestFixture]
  public class RuleSystemUnitTest
  {
    class DummyFact : Fact {}
    class AnotherDummyFact : Fact { }

    class FactHandlerTest : FactHandler {

      public void Handle(DummyFact fact)
      {
        int o = 0;
      }
      public void Handle(AnotherDummyFact fact)
      {
        int o = 0;
      }
    }
    
    [Test]
    public void TestFactHandling()
    {
      //var system = new ComponentSystem();
      //var handlers = new FactHandlerTest();
      //var rs = new RuleSystem<String>(null);
      //var entity = EntityBuilder.Build().With(system, 
      //        new RuleSystemComponent<String>(rs, handlers)).GetEntity();

      //rs.Add(new Rule<string>( x=>true, s=>{ 
        
      //} ));

    }


  }
}
