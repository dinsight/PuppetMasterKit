using System;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Tilemap;
using SpriteKit;

namespace PuppetMasterKit.Template.Game.Ios.Bindings
{
  public static class Registration
  {
    public static void RegisterBindings(SKScene scene)
    {
      Container.GetContainer().Register<ISpriteFactory>(factory => new SpriteFactory(scene));
      Container.GetContainer().Register<ICoordinateMapper>(factory => new IsometricMapper(scene));
      Container.GetContainer().Register<SKScene>(factory => scene);
    }

    public static void Register<T>(T instance)
    {
      Container.GetContainer().Register<T>(factory => instance);
    }
  }
}
