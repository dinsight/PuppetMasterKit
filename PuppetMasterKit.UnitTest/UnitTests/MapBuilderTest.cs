
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

    
    private static void PrintMap(I2DSubscript<int?> i2)
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

    
  }
}
