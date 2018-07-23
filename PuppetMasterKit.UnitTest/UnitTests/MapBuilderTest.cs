
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

      builder.AddRoom(modules[0], 158, 76);
            builder.AddRoom(modules[2], 85, 156);
            builder.AddRoom(modules[0], 73, 150);
            builder.AddRoom(modules[2], 155, 201);
            builder.AddRoom(modules[1], 67, 227);
            builder.AddRoom(modules[2], 51, 54);
            builder.AddRoom(modules[0], 146, 7);
            builder.AddRoom(modules[0], 129, 36);
            builder.AddRoom(modules[1], 209, 220);
            builder.AddRoom(modules[1], 142, 217);
            builder.AddRoom(modules[2], 25, 152);
            builder.AddRoom(modules[1], 18, 123);
            builder.AddRoom(modules[0], 102, 50);
            builder.AddRoom(modules[1], 172, 102);
            builder.AddRoom(modules[2], 47, 175);
            builder.AddRoom(modules[1], 93, 194);
            builder.AddRoom(modules[0], 93, 130);
            builder.AddRoom(modules[0], 138, 53);
            builder.AddRoom(modules[0], 205, 35);
            builder.AddRoom(modules[2], 42, 132);
            builder.AddRoom(modules[1], 8, 136);
            builder.AddRoom(modules[2], 17, 49);
            builder.AddRoom(modules[2], 12, 157);
            builder.AddRoom(modules[1], 111, 106);
            builder.AddRoom(modules[0], 84, 110);
            builder.AddRoom(modules[1], 57, 161);
            builder.AddRoom(modules[0], 14, 80);
            builder.AddRoom(modules[1], 139, 161);
            builder.AddRoom(modules[2], 88, 175);
            builder.AddRoom(modules[2], 19, 107);
            builder.AddRoom(modules[0], 166, 207);
            builder.AddRoom(modules[2], 78, 15);
            builder.AddRoom(modules[1], 113, 138);
            builder.AddRoom(modules[1], 187, 94);
            builder.AddRoom(modules[0], 84, 221);
            builder.AddRoom(modules[1], 111, 213);
            builder.AddRoom(modules[2], 122, 68);
            builder.AddRoom(modules[2], 208, 183);
            builder.AddRoom(modules[2], 63, 51);
            builder.AddRoom(modules[0], 193, 14);
            builder.AddRoom(modules[1], 41, 43);
            builder.AddRoom(modules[0], 104, 155);
            builder.AddRoom(modules[2], 193, 41);
            builder.AddRoom(modules[0], 43, 24);
            builder.AddRoom(modules[1], 65, 14);
            builder.AddRoom(modules[1], 29, 171);
            builder.AddRoom(modules[2], 152, 58);
            builder.AddRoom(modules[1], 228, 61);
            builder.AddRoom(modules[1], 125, 231);
            builder.AddRoom(modules[1], 169, 188);
            builder.AddRoom(modules[0], 68, 185);
            builder.AddRoom(modules[1], 15, 179);
            builder.AddRoom(modules[0], 168, 160);
            builder.AddRoom(modules[1], 146, 36);
            builder.AddRoom(modules[0], 120, 53);
            builder.AddRoom(modules[2], 109, 79);
            builder.AddRoom(modules[0], 131, 125);
            builder.AddRoom(modules[0], 61, 65);
            builder.AddRoom(modules[1], 216, 83);
            builder.AddRoom(modules[2], 189, 58);
            builder.AddRoom(modules[0], 154, 149);
            builder.AddRoom(modules[2], 62, 199);
            builder.AddRoom(modules[1], 55, 13);
            builder.AddRoom(modules[0], 40, 54);
            builder.AddRoom(modules[0], 123, 152);
            builder.AddRoom(modules[1], 114, 192);
            builder.AddRoom(modules[0], 218, 21);
            builder.AddRoom(modules[1], 38, 89);
            builder.AddRoom(modules[0], 47, 112);
            builder.AddRoom(modules[2], 24, 93);
            builder.AddRoom(modules[1], 193, 127);
            builder.AddRoom(modules[0], 25, 210);
            builder.AddRoom(modules[0], 131, 217);
            builder.AddRoom(modules[2], 58, 97);
            builder.AddRoom(modules[0], 157, 189);
            builder.AddRoom(modules[2], 143, 181);
            builder.AddRoom(modules[0], 119, 127);
            builder.AddRoom(modules[1], 101, 29);
            builder.AddRoom(modules[0], 105, 201);
            builder.AddRoom(modules[1], 48, 201);
            builder.AddRoom(modules[2], 13, 10);
            builder.AddRoom(modules[0], 57, 137);
            builder.AddRoom(modules[0], 179, 114);
            builder.AddRoom(modules[0], 196, 214);
            builder.AddRoom(modules[1], 36, 212);
            builder.AddRoom(modules[0], 22, 195);
            builder.AddRoom(modules[2], 163, 88);
            builder.AddRoom(modules[0], 225, 202);
            builder.AddRoom(modules[0], 121, 101);
            builder.AddRoom(modules[1], 163, 35);
            builder.AddRoom(modules[0], 196, 156);
            builder.AddRoom(modules[0], 175, 226);
            builder.AddRoom(modules[2], 183, 206);
            builder.AddRoom(modules[2], 158, 132);
            builder.AddRoom(modules[1], 232, 228);
            builder.AddRoom(modules[0], 208, 72);
            builder.AddRoom(modules[1], 48, 220);
            builder.AddRoom(modules[0], 50, 39);
            builder.AddRoom(modules[2], 130, 23);
            builder.AddRoom(modules[1], 219, 155);
            builder.AddRoom(modules[0], 224, 100);
            builder.AddRoom(modules[1], 70, 89);
            builder.AddRoom(modules[0], 120, 7);
            builder.AddRoom(modules[0], 118, 29);
            builder.AddRoom(modules[1], 66, 133);
            builder.AddRoom(modules[1], 212, 59);
            builder.AddRoom(modules[1], 88, 86);
            builder.AddRoom(modules[0], 89, 27);
            builder.AddRoom(modules[2], 193, 169);
            builder.AddRoom(modules[1], 174, 32);
            builder.AddRoom(modules[0], 218, 191);
            builder.AddRoom(modules[0], 98, 227);
            builder.AddRoom(modules[0], 102, 187);
            builder.AddRoom(modules[0], 141, 229);
            builder.AddRoom(modules[2], 205, 113);
            builder.AddRoom(modules[0], 215, 32);
            builder.AddRoom(modules[1], 169, 137);
            builder.AddRoom(modules[0], 164, 56);
            builder.AddRoom(modules[0], 139, 78);
            builder.AddRoom(modules[0], 196, 72);
            builder.AddRoom(modules[1], 70, 70);
            builder.AddRoom(modules[0], 192, 146);
            builder.AddRoom(modules[0], 94, 100);
            builder.AddRoom(modules[2], 156, 220);
            builder.AddRoom(modules[0], 205, 11);
            builder.AddRoom(modules[0], 8, 193);
            builder.AddRoom(modules[2], 66, 35);
            builder.AddRoom(modules[0], 33, 26);
            builder.AddRoom(modules[2], 175, 10);
      builder.CreatePaths();

      PrintMap(builder, modules);
    }

  }
}
