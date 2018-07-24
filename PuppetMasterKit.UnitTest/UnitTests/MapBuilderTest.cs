
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
    static int rows = 150;
    static int cols = 150;

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
          //var c = (char)(pathCh + x - MapCodes.PATH);
          var c = 'A';
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
      
      var roomCount = 300;
      var actual = 0;
      var start = DateTime.Now;
      //actual = builder.Create(roomCount, modules);

      //var r1 = builder.AddRoom(module1, 25, 40);
      //var r2 = builder.AddRoom(module1, 37, 35);
      //var r3 = builder.AddRoom(module1, 15, 45);
      //var r4 = builder.AddRoom(module1, 40, 25);
      //var r5 = builder.AddRoom(module1, 20, 65);
      //builder.CreatePaths();
      //builder.CreatePath(r1, r2);

      builder.AddRoom(modules[1], 8, 125);
      builder.AddRoom(modules[0], 55, 38);
      builder.AddRoom(modules[2], 64, 47);
      builder.AddRoom(modules[0], 118, 7);
      builder.AddRoom(modules[2], 53, 108);
      builder.AddRoom(modules[0], 27, 124);
      builder.AddRoom(modules[0], 38, 39);
      builder.AddRoom(modules[0], 86, 25);
      builder.AddRoom(modules[2], 60, 130);
      builder.AddRoom(modules[0], 97, 99);
      builder.AddRoom(modules[2], 128, 128);
      builder.AddRoom(modules[0], 6, 30);
      builder.AddRoom(modules[1], 64, 76);
      builder.AddRoom(modules[1], 82, 126);
      builder.AddRoom(modules[2], 107, 49);
      builder.AddRoom(modules[2], 18, 47);
      builder.AddRoom(modules[2], 139, 89);
      builder.AddRoom(modules[0], 30, 52);
      builder.AddRoom(modules[1], 80, 49);
      builder.AddRoom(modules[1], 132, 52);
      builder.AddRoom(modules[0], 137, 76);
      builder.AddRoom(modules[0], 45, 28);
      builder.AddRoom(modules[0], 22, 101);
      builder.AddRoom(modules[1], 116, 31);
      builder.AddRoom(modules[1], 104, 111);
      builder.AddRoom(modules[0], 98, 138);
      builder.AddRoom(modules[0], 51, 64);
      builder.AddRoom(modules[0], 7, 136);
      builder.AddRoom(modules[0], 39, 117);
      builder.AddRoom(modules[1], 69, 92);
      builder.AddRoom(modules[1], 121, 87);
      builder.AddRoom(modules[2], 40, 98);
      builder.AddRoom(modules[2], 95, 61);
      builder.AddRoom(modules[1], 64, 21);
      builder.AddRoom(modules[2], 79, 71);
      builder.AddRoom(modules[0], 71, 104);
      builder.AddRoom(modules[2], 24, 19);
      builder.AddRoom(modules[2], 53, 76);
      builder.AddRoom(modules[1], 116, 133);
      builder.AddRoom(modules[2], 40, 130);
      builder.AddRoom(modules[1], 97, 77);
      builder.AddRoom(modules[1], 42, 79);
      builder.AddRoom(modules[2], 117, 111);
      builder.AddRoom(modules[2], 112, 63);
      builder.AddRoom(modules[2], 93, 114);
      builder.AddRoom(modules[1], 97, 27);
      builder.AddRoom(modules[0], 94, 46);
      builder.AddRoom(modules[0], 79, 140);
      builder.AddRoom(modules[0], 91, 13);
      builder.AddRoom(modules[2], 76, 32);
      builder.AddRoom(modules[1], 85, 98);
      builder.AddRoom(modules[1], 13, 78);
      builder.AddRoom(modules[0], 71, 132);
      builder.AddRoom(modules[2], 127, 20);
      builder.AddRoom(modules[2], 28, 67);
      builder.AddRoom(modules[0], 47, 52);
      builder.AddRoom(modules[0], 71, 9);
      builder.AddRoom(modules[1], 16, 112);
      builder.AddRoom(modules[2], 109, 79);
      builder.AddRoom(modules[1], 13, 62);
      builder.AddRoom(modules[1], 108, 11);
      builder.AddRoom(modules[0], 142, 125);
      builder.AddRoom(modules[2], 54, 91);
      builder.AddRoom(modules[0], 96, 126);
      builder.AddRoom(modules[1], 131, 104);
      builder.AddRoom(modules[2], 41, 11);
      builder.AddRoom(modules[0], 143, 7);
      builder.AddRoom(modules[2], 140, 26);
      builder.AddRoom(modules[0], 23, 134);
      builder.CreatePaths();

      var end = DateTime.Now;
      PrintMap(builder, modules);
      Console.WriteLine($"Created {actual} out of {roomCount} in {(end-start).TotalMilliseconds} ms");

    }

  }
}
