using System;
using System.Collections.Generic;

namespace PuppetMasterKit.Utility
{
  public static class CollectionExtension
  {
    /// <summary>
    /// Fors the each.
    /// </summary>
    /// <param name="collection">Collection.</param>
    /// <param name="action">Action.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static void ForEach<T>(this ICollection<T> collection, Action<T> action)
    {
      foreach (var item in collection) {
        action(item);
      }
    }
  }
}
