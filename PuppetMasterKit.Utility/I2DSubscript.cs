using System;
namespace PuppetMasterKit.Utility
{
  public interface I2DSubscript<T>
  {
    T this[int row, int col] { get; }
  }
}
