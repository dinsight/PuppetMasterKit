
using System;
using System.Collections.Generic;

namespace PuppetMasterKit.Utility
{
    public static class EnumerableExtension
	{
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }
	}
}
