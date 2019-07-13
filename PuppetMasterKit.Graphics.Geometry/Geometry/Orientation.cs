using System;
using PuppetMasterKit.Utility.Extensions;

namespace PuppetMasterKit.Graphics.Geometry
{
  public class Orientation
  {
    #region Orientations
    public const string N = "n";
    public const string S = "s";
    public const string E = "e";
    public const string W = "w";
    public const string NE = "ne";
    public const string NW = "nw";
    public const string SW = "sw";
    public const string SE = "se";
    #endregion
    /// <summary>
    /// Gets the sprite orientation.
    /// </summary>
    /// <returns>The sprite orientation.</returns>
    /// <param name="direction">Direction.</param>
    public static string GetOrientation(Vector direction)
    {
      if (direction.Magnitude().IsZero())
        return null;

      var zero = 0f;
      var bracketPI6 = (float)Math.PI / 8;
      var bracketPI3 = (float)(3 * Math.PI / 8);
      var bracketPI2 = (float)(Math.PI / 2);

      if (!direction.Dx.IsZero()) {
        var absVal = (float)Math.Abs(direction.Dy / direction.Dx);
        var angle = (float)Math.Atan(absVal);

        if (angle.IsBetween(zero, bracketPI6)) {
          if (direction.Dx > 0) {
            return E;
          } else {
            return W;
          }
        }
        if (angle.IsBetween(bracketPI6, bracketPI3)) {
          if (direction.Dx > 0 && direction.Dy > 0) {
            return NE;
          }
          if (direction.Dx > 0 && direction.Dy < 0) {
            return SE;
          }
          if (direction.Dx < 0 && direction.Dy > 0) {
            return NW;
          }
          if (direction.Dx < 0 && direction.Dy < 0) {
            return SW;
          }
        }
        if (angle.IsBetween(bracketPI3, bracketPI2)) {
          if (direction.Dy > 0) {
            return N;
          } else {
            return S;
          }
        }
      } else {
        if (direction.Dy > 0) {
          return N;
        } else {
          return S;
        }
      }

      return null;
    }
  }
}
