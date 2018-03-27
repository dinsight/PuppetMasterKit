using System;

namespace  PuppetMasterKit.Utility
{
    public static class ArrayExtension
    {
        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            foreach (var item in array)
            {
                action(item);
            }
        }
    }
}
