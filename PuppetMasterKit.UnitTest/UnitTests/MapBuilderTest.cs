
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Map;

namespace PuppetMasterKit.UnitTest
{
  [TestFixture]
  public class MapBuilderTest
  {
    static int rows = 50;
    static int cols = 100;

    /// <summary>
    /// Prints the map.
    /// </summary>
    /// <param name="builder">Builder.</param>
    private static void PrintMap(MapBuilder builder, List<Module> modules)
    {
      var pathCh = 'A';
      var buffer = new StringBuilder();
      var line = new StringBuilder();
      line.Append("    ");
      for (int i = 0; i < cols; i++) {
        line.Append((i % 10).ToString("D1"));
      }
      buffer.Append(line.ToString());
      buffer.AppendLine();
      line.Length = 0;
      line.Append("000 ");
      builder.Apply((i, j, x) => {
        if (x == MapBuilder.Blank) {
          line.Append("*");
        } else if (x >= MapCodes.PATH) {
          var c = (char)(pathCh + x - MapCodes.PATH);
          line.Append(c);
        } else {
          line.Append(x.ToString());
        }
        if (j == cols - 1) {
          line.AppendLine();
          buffer.Append(line.ToString());
          line.Length = 0;
          line.Append((i + 1).ToString("D3") + " ");
        }
      });
      Console.WriteLine(buffer.ToString());

      builder.Rooms.ForEach(x => {
        Console.WriteLine($"builder.AddRoom(modules[{modules.IndexOf(x.Module)}],{x.Row},{x.Col});");
      });
    }

    [Test]
    public void GenerateMap()
    {
      var C = MapCodes.CENTER;
      var E = MapCodes.EXIT;
      var builder = new MapBuilder(rows, cols, 5, new PathFinder());
      var modules = new List<Module>();

      var module0 = new Module( new int[,] { 
                { 1,1,1,1,1,},
                { 1,1,C,1,1,},
                { 1,1,1,1,1,},
                
      });
      var module1 = new Module(new int[,] {
                { 1,1,1,1,1,1,1,},
                { 1,1,1,1,1,1,1,},
                { 1,1,1,1,C,1,1,},
                { 1,1,1,1,1,1,1,},
                { 1,1,1,1,1,1,1,},
            });

      var module2 = new Module(new int[,] {
                { 0,0,0,1,0,0,0,0,0 },
                { 1,1,1,1,1,1,1,1,1 },
                { 1,1,1,1,1,1,1,1,1 },
                { 1,1,1,1,C,1,1,1,1 },
                { 1,1,1,1,1,1,1,1,1 },
                { 1,1,1,1,1,1,1,1,1 },
                { 1,1,1,1,1,1,1,1,1 },
            });

      modules.Add(module0);
      modules.Add(module1);
      modules.Add(module2);

      var roomCount = 100;
      //var actual = builder.Create(roomCount, modules);
      //Console.WriteLine($"Created {actual} out of {roomCount}");

      //var r1 = builder.AddRoom(module1, 25, 40);
      //var r2 = builder.AddRoom(module1, 37, 35);
      //var r3 = builder.AddRoom(module1, 15, 45);
      //var r4 = builder.AddRoom(module1, 40, 25);
      //var r5 = builder.AddRoom(module1, 20, 65);
      //builder.CreatePaths();
      //builder.CreatePath(r1, r2);

      builder.AddRoom(modules[2], 14, 59);
      builder.AddRoom(modules[2], 23, 17);
      builder.AddRoom(modules[0], 38, 11);
      builder.AddRoom(modules[1], 25, 68);
      builder.AddRoom(modules[0], 13, 25);
      builder.AddRoom(modules[2], 36, 58);
      builder.AddRoom(modules[1], 40, 83);
      builder.AddRoom(modules[0], 24, 54);
      builder.AddRoom(modules[1], 18, 38);
      builder.AddRoom(modules[1], 10, 79);
      builder.AddRoom(modules[2], 28, 81);
      builder.AddRoom(modules[1], 41, 30);
      builder.CreatePaths();

      PrintMap(builder, modules);
    }

  }
}
