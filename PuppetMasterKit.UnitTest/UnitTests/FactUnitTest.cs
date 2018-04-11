
using System;
using NUnit.Framework;

namespace PuppetMasterKit.UnitTest
{
  [TestFixture]
  public class FactUnitTest
  {
    class CustomFact1 : AI.Rules.Fact
    {
      
    }

    class CustomFact2 : AI.Rules.Fact
    {

    }
    
    [Test]
    public void TestEquals()
    {
      var fact0 = new CustomFact1();
      var fact1 = new CustomFact1();
      var fact2 = new CustomFact2();

      Assert.True(fact0 == fact1);
      Assert.False(fact1 == fact2);
    }

  }
}
