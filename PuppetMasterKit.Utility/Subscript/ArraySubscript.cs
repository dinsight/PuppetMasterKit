using System;
namespace PuppetMasterKit.Utility.Subscript
{
  /// <summary>
  /// Helper for consuming 2D arrays
  /// </summary>
  public class ArraySubscript<T> : I2DSubscript<T?> where T : struct
  {
    readonly T[,] array;
    readonly int rows;
    readonly int cols;

    public int Rows => rows;

    public int Cols => cols;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Terrain.Map.ArraySubscript"/> class.
    /// </summary>
    /// <param name="array">Array.</param>
    public ArraySubscript(T[,] array)
    {
      this.array = array;
      rows = array.GetLength(0);
      cols = array.GetLength(1);
    }

    /// <summary>
    /// Gets the <see cref="T:PuppetMasterKit.Utility.ArraySubscript`1"/> with the specified row col.
    /// </summary>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    public T? this[int row, int col] {
      get {
        if (row >= 0 && col >= 0 && row < rows && col < cols) {
          return array[row, col];
        }
        return default(T);
      }
    }
  }
}

