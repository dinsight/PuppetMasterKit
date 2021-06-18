using System;
using System.Collections.Generic;

namespace PuppetMasterKit.Utility.Extensions
{
  public static class MatrixExtension
  {
    public class MatrixElement {
      public int Row { get; set; }
      public int Col { get; set; }
      public int Value { get; set; }
    }

    public static IEnumerable<MatrixElement> GetEnumerable(this int[,] matrix) {
      for (int i = 0; i < matrix.GetLength(0); i++) {
        for (int j = 0; j < matrix.GetLength(1); j++) {
          yield return new MatrixElement() { Row = i, Col = j, Value = matrix[i,j] };
        }
      }
    }
  }
}
