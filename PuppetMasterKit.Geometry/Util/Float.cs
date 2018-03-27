using System;
namespace PuppetMasterKit.Geometry.Util
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
    }
}
