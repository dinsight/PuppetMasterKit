using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;
using PuppetMasterKit.Terrain.Map;
using System.Collections.Generic;
using PuppetMasterKit.Terrain.Map.SimplePlacement;
using System;
using PuppetMasterKit.Terrain.Map.CellularAutomata;

namespace PuppetMasterKit.Template.Game.Ios.Bindings
{
  public static class Registration
  {
    public static void RegisterBindings(SKScene scene)
    {
      Container.GetContainer().Register<ISpriteFactory>(factory => new SpriteFactory(scene));
      Container.GetContainer().Register<ICoordinateMapper>(factory => new IsometricMapper(scene));
      Container.GetContainer().Register<SKScene>(factory => scene);

      //Container.GetContainer().Register<IMapGenerator>(factory =>
      //new MapBuilder(120, CreateAvailableModules(), 5, new PathFinder()));

      Random random = new Random(1);

      Container.GetContainer().Register<IMapGenerator>(factory => 
        new CellularAutomataGenerator(12, 5,4,
        (i, j, rows, cols) => {
          if (i == 0 || i == rows - 1 || j == 0 || j == cols - 1)
            return 1;
          if (random.Next(1, 101) < 48) {
            return 1;
          }
          return 0;
        })
      );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    public static void Register<T>(T instance)
    {
      Container.GetContainer().Register<T>(factory => instance);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private static List<Module> CreateAvailableModules()
    {
      var modules = new List<Module>();

      var module0 = new Module(new int[,] {
                { 3,3,3,3,3},
                { 3,3,3,3,3},
                { 3,3,3,3,3},
                { 3,3,3,3,3},
      }, '+');

      var module1 = new Module(new int[,] {
                { 1,1,1,1,1,1,1},
                { 1,1,1,1,1,1,1},
                { 1,1,1,1,1,1,1},
                { 1,1,1,1,1,1,1},
                { 1,1,1,1,1,1,1},
                { 1,1,1,1,1,1,1},
                { 1,1,1,1,1,1,1},
      }, 1) { IsAccessible = false };

      var module2 = new Module(new int[,] {
                { 0,0,0,3,0,0,0,0,0 },
                { 3,3,3,3,3,3,3,3,3 },
                { 3,3,3,3,3,3,3,3,3 },
                { 3,3,3,3,3,3,3,3,3 },
                { 3,3,3,3,3,3,3,3,3 },
                { 3,3,3,3,3,3,3,3,3 },
                { 3,3,3,3,3,3,3,3,3 },
            }, 'W');

      modules.Add(module0);
      modules.Add(module1);
      modules.Add(module2);
      return modules;
    }
  }
}
