using System;

namespace PuppetMasterKit.Utility.Extensions
{
  public static class ArrayExtension
  {
    public static void ForEach<T>(this T[] array, Action<T> action)
    {
      foreach (var item in array) {
        action(item);
      }
    }

    public static void ForEach<T>(this T[] array, Action<int,T> action)
    {
      for (int index = 0; index < array.Length; index++) {
        action(index, array[index]);
      }
    }
  }
}
