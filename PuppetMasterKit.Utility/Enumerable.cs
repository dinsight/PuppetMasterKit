
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

    /// <summary>
    /// Minimums the by.
    /// </summary>
    /// <returns>The by.</returns>
    /// <param name="source">Source.</param>
    /// <param name="selector">Selector.</param>
    /// <typeparam name="TSource">The 1st type parameter.</typeparam>
    /// <typeparam name="TKey">The 2nd type parameter.</typeparam>
    public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
    {
      return source.MinBy(selector, null);
    }

    /// <summary>
    /// Minimums the by.
    /// </summary>
    /// <returns>The by.</returns>
    /// <param name="source">Source.</param>
    /// <param name="selector">Selector.</param>
    /// <param name="comparer">Comparer.</param>
    /// <typeparam name="TSource">The 1st type parameter.</typeparam>
    /// <typeparam name="TKey">The 2nd type parameter.</typeparam>
    public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector, IComparer<TKey> comparer)
    {
      if (source == null) throw new ArgumentNullException("source");
      if (selector == null) throw new ArgumentNullException("selector");
      comparer = comparer ?? Comparer<TKey>.Default;

      using (var sourceIterator = source.GetEnumerator()) {
        if (!sourceIterator.MoveNext()) {
          throw new InvalidOperationException("Sequence contains no elements");
        }
        var min = sourceIterator.Current;
        var minKey = selector(min);
        while (sourceIterator.MoveNext()) {
          var candidate = sourceIterator.Current;
          var candidateProjected = selector(candidate);
          if (comparer.Compare(candidateProjected, minKey) < 0) {
            min = candidate;
            minKey = candidateProjected;
          }
        }
        return min;
      }
    }
  }
}
