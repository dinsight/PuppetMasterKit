
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PuppetMasterKit.Terrain.Map.SimplePlacement;

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
  }
}
