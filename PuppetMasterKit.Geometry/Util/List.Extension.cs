﻿using System;
using System.Collections.Generic;

namespace PuppetMasterKit.Geometry.Util
{
    public static class ListExtension
	{
		
        /// <summary>
        /// Deduplicate the specified list.
        /// </summary>
        /// <returns>The deduplicate.</returns>
        /// <param name="list">List.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> Deduplicate<T>(this List<T> list ) 
            where T : class{

            var result = new List<T>();

            for (int index = 0; index < list.Count; index++)
            {
                var curr = list[index];
                var isDupe = false;

                for (int k = 0; k < index; k++)
                {
                    var prev = list[k];

                    if (prev.Equals(curr))
                    {
                        isDupe = true;
                        break;
                    }
                }

                if (!isDupe) {
                    result.Add(curr);
                }
            }
            return result;
        }
	}
}
