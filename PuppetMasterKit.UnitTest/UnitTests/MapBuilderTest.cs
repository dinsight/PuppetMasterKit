
using System;
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
    static int cols = 50;

    /// <summary>
    /// Prints the map.
    /// </summary>
    /// <param name="builder">Builder.</param>
    private static void PrintMap(MapBuilder builder)
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
        line.Append(x == MapBuilder.Blank ? "*" : x.ToString());
        if (j == cols - 1) {
          line.AppendLine();
          buffer.Append(line.ToString());
          line.Length = 0;
          line.Append((i + 1).ToString("D3") + " ");
        }
      });
      Console.WriteLine(buffer.ToString());
      //Console.ReadKey();
    }

    [Test]
    public void GenerateMap()
    {
      var C = MapCodes.CENTER;
      var E = MapCodes.EXIT;
      var builder = new MapBuilder(rows, cols, 2, new PathFinder());
      var modules = new List<Module>();

      var module1 = new Module(new int[,] {
                { 0,0,1,1,E,1,1,1,0,0 },
                { 0,1,1,1,1,1,1,1,1,0 },
                { 1,1,1,1,1,1,1,1,1,1 },
                { 1,1,1,1,1,C,1,1,1,1 },
                { 0,0,0,1,1,1,1,1,1,1 },
                { 0,0,0,1,1,1,1,1,1,0 },
                { 0,0,0,1,E,1,1,1,1,0 },
            });

      var module2 = new Module(new int[,] {
                { 0,0,0,0,0,E,0,0,0,0 },
                { 0,0,0,0,1,1,1,0,0,0 },
                { 0,0,1,1,1,1,1,1,1,0 },
                { E,1,1,1,1,C,1,1,0,0 },
                { 0,0,0,1,1,1,1,1,0,0 },
                { 0,0,0,1,1,1,1,1,1,0 },
                { 0,0,0,0,0,0,0,0,0,0 },
            });

      modules.Add(module1);
      modules.Add(module2);
      var roomCount = 30;
      var actual = builder.Create(roomCount, modules);
      Console.WriteLine($"Created {actual} out of {roomCount}");
      PrintMap(builder);
    }

  }
}
