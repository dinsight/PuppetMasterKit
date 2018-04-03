﻿
using System;
using System.Collections.Generic;

namespace PuppetMasterKit.Utility
{
  public static class EnumerableExtension
  {
    /// <summary>
    /// Fors the each.
    /// </summary>
    /// <param name="list">List.</param>
    /// <param name="action">Action.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
    {
      foreach (var item in list) {
        action(item);
      }
    }
  }
}
