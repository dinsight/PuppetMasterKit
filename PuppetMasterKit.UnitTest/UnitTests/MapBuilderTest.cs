
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using PuppetMasterKit.Terrain.Map.SimplePlacement;
using PuppetMasterKit.Terrain.Map.CellularAutomata;
using PuppetMasterKit.Utility.Subscript;

namespace PuppetMasterKit.UnitTest
{
  [TestFixture]
  public class MapBuilderTest
  {
    static int rows = 140;
    static int cols = 140;

    
    private static void PrintMap(I2DSubscript<int> i2)
    {
      for (int i = 0; i < i2.Rows; i++) {
        for (int j = 0; j < i2.Cols; j++) {
          var x = i2[i, j];
          if (x == 0) {
            Console.Write("∙");
          } else if (x == 1) {
            Console.Write("#");
          } else if (x == 3) {
            Console.Write("~");
          } else {
            Console.Write((char)x);
          }
        }
        Console.WriteLine();
      }
    }

    [Test]
    public void TestCA() {
      Random random = new Random(13);
      var gen = new CellularAutomataGenerator(30, 5, 4,
        (i, j, rows, cols) => {
          if (i == 0 || i == rows - 1 || j == 0 || j == cols - 1)
            return 1;
          if (random.Next(1, 101) < 50) {
            return 1;
          }
          return 0;
        });

      var regs = gen.Create(100, 100);
      PrintMap(gen);
    }
  }
}
