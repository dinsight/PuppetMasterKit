using System;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.UnitTest.Test.Bindings;
using SpriteKit;

namespace PuppetMasterKit.Template.Test.Bindings
{
  public static class Registration
  {
    public static void RegisterBindings()
    {
      Container.GetContainer().Register<ISpriteFactory>(factory => new SpriteFactory());
      Container.GetContainer().Register<ICoordinateMapper>(factory => new IdentityMapper());
    }
  }
}
