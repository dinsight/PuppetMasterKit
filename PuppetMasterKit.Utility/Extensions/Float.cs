﻿using System;
namespace PuppetMasterKit.Utility.Extensions
{
    public static class Float
    {
        public static int Decimals = 4;

        public static float EPSILON = 1E-2f;

        /// <summary>
        /// Ises the between number, a and b.
        /// </summary>
        /// <returns>The <see cref="T:System.Boolean"/>.</returns>
        /// <param name="number">Number.</param>
        /// <param name="a">The alpha component.</param>
        /// <param name="b">The blue component.</param>
        public static bool IsBetween(this float number, float a, float b) 
        {
            return number >= a && number <= b;
        }

        /// <summary>
        /// Ises the zero.
        /// </summary>
        /// <returns><c>true</c>, if zero was ised, <c>false</c> otherwise.</returns>
        /// <param name="number">Number.</param>
        public static bool IsZero(this float number)
        {
            return Math.Abs(number) < Float.EPSILON;
        }

        /// <summary>
        /// Equals the specified number and otherNumber.
        /// </summary>
        /// <returns>The equals.</returns>
        /// <param name="number">Number.</param>
        /// <param name="otherNumber">Other number.</param>
        public static bool Equals(this float number, float otherNumber)
        {
            return Math.Abs(number - otherNumber) < Float.EPSILON;
        }
    }
}
