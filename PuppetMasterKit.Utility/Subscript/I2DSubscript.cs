using System;
namespace PuppetMasterKit.Utility.Subscript
{
  public interface I2DSubscript<T>
  {
    int Rows { get; }
    int Cols { get; }
    T this[int row, int col] { get; }
  }
}
