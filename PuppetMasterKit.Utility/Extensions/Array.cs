using System;

namespace PuppetMasterKit.Utility.Extensions
{
  public static class ArrayExtension
  {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="action"></param>
    public static void ForEach<T>(this T[] array, Action<T> action)
    {
      foreach (var item in array) {
        action(item);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="action"></param>
    public static void ForEach<T>(this T[] array, Action<int,T> action)
    {
      for (int index = 0; index < array.Length; index++) {
        action(index, array[index]);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="value"></param>
    public static void Reset<T>(this T[,] array, T value = default) {
      var dim1 = array.GetLength(0);
      var dim2 = array.GetLength(1);
      for (int i = 0; i < dim1; i++) {
        for (int j = 0; j < dim2; j++) {
          array[i, j] = value;
        }
      }
    }
  }
}
