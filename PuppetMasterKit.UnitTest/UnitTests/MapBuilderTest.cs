
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
    static int rows = 140;
    static int cols = 140;

    /// <summary>
    /// Prints the map.
    /// </summary>
    /// <param name="builder">Builder.</param>
    private static void PrintMap(MapBuilder builder, List<Module> modules)
    {
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
          line.Append("∙");
        } else if (x == MapCodes.X) { 
          line.Append("@");
        } else if (x == MapCodes.PATH) {
          line.Append(' ');
        } else {
          line.Append((char)x);
        }
        if (j == cols - 1) {
          line.AppendLine();
          buffer.Append(line.ToString());
          line.Length = 0;
          line.Append((i + 1).ToString("D3") + " ");
        }
      });
      Console.WriteLine(buffer.ToString());
      foreach (var x in builder.Rooms) {
        Console.WriteLine($"builder.AddRoom(modules[{modules.IndexOf(x.Module)}],{x.Row},{x.Col});");
      }
    }

    [Test]
    public void GenerateMap()
    {
      var builder = new MapBuilder(rows, cols, 5, new PathFinder());
      var modules = new List<Module>();

      var module0 = new Module( new int[,] { 
                { 1,1,1,1,1,},
                { 1,1,1,1,1,},
                { 1,1,1,1,1,},
                
      }, '-');
      var module1 = new Module(new int[,] {
                { 1,1,1,1,1,1,1,},
                { 1,1,1,1,1,1,1,},
                { 1,1,1,1,1,1,1,},
                { 1,1,1,1,1,1,1,},
                { 1,1,1,1,1,1,1,},
            }, '+');

      var module2 = new Module(new int[,] {
                { 0,0,0,1,0,0,0,0,0 },
                { 1,1,1,1,1,1,1,1,1 },
                { 1,1,1,1,1,1,1,1,1 },
                { 1,1,1,1,1,1,1,1,1 },
                { 1,1,1,1,1,1,1,1,1 },
                { 1,1,1,1,1,1,1,1,1 },
                { 1,1,1,1,1,1,1,1,1 },
            }, '|');

      modules.Add(module0);
      modules.Add(module1);
      modules.Add(module2);
      
      var roomCount = 100;
      var actual = 0;
      var start = DateTime.Now;
      actual = builder.Create(roomCount, modules);
      /*
      builder.AddRoom(modules[2], 111, 110);
      builder.AddRoom(modules[2], 69, 112);
      builder.AddRoom(modules[2], 34, 123);
      builder.AddRoom(modules[0], 108, 58);
      builder.AddRoom(modules[0], 87, 14);
      builder.AddRoom(modules[0], 126, 34);
      builder.AddRoom(modules[2], 111, 27);
      builder.AddRoom(modules[2], 95, 129);
      builder.AddRoom(modules[1], 25, 28);
      builder.AddRoom(modules[0], 52, 37);
      builder.AddRoom(modules[1], 51, 10);
      builder.AddRoom(modules[1], 61, 91);
      builder.AddRoom(modules[2], 55, 66);
      builder.AddRoom(modules[2], 23, 64);
      builder.AddRoom(modules[1], 66, 25);
      builder.AddRoom(modules[2], 129, 96);
      builder.AddRoom(modules[0], 8, 125);
      builder.AddRoom(modules[1], 37, 55);
      builder.AddRoom(modules[0], 83, 62);
      builder.AddRoom(modules[2], 100, 41);
      builder.AddRoom(modules[0], 14, 12);
      builder.AddRoom(modules[0], 81, 116);
      builder.AddRoom(modules[0], 61, 79);
      builder.AddRoom(modules[2], 125, 78);
      builder.AddRoom(modules[2], 14, 45);
      builder.AddRoom(modules[1], 110, 13);
      builder.AddRoom(modules[1], 47, 94);
      builder.AddRoom(modules[1], 104, 90);
      builder.AddRoom(modules[1], 74, 41);
      builder.AddRoom(modules[0], 114, 39);
      builder.AddRoom(modules[0], 47, 79);
      builder.AddRoom(modules[2], 117, 127);
      builder.AddRoom(modules[0], 43, 32);
      builder.AddRoom(modules[0], 73, 90);
      builder.AddRoom(modules[2], 120, 51);
      builder.AddRoom(modules[1], 23, 9);
      builder.AddRoom(modules[1], 31, 107);
      builder.CreatePaths();
      builder.ParitionMap();
      */

      var end = DateTime.Now;
      PrintMap(builder, modules);
      Console.WriteLine($"Created {actual} out of {roomCount} in {(end-start).TotalMilliseconds} ms");

    }

  }
}
