using System;
using PuppetMasterKit.Utility.Attributes;
using PuppetMasterKit.Utility.Extensions;

namespace PuppetMasterKit.Graphics.Geometry
{
  public enum Orientation {
    [StringValue("n")] N,
    [StringValue("s")] S,
    [StringValue("e")] E,
    [StringValue("w")] W,
    [StringValue("ne")] NE,
    [StringValue("nw")] NW,
    [StringValue("se")] SE,
    [StringValue("sw")] SW
  }

  public static class OrientationExtension
  {
    /// <summary>
    /// Gets the sprite orientation.
    /// </summary>
    /// <returns>The sprite orientation.</returns>
    /// <param name="direction">Direction.</param>
    public static Orientation? GetOrientation(Vector direction)
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
            return Orientation.E;
          } else {
            return Orientation.W;
          }
        }
        if (angle.IsBetween(bracketPI6, bracketPI3)) {
          if (direction.Dx > 0 && direction.Dy > 0) {
            return Orientation.NE;
          }
          if (direction.Dx > 0 && direction.Dy < 0) {
            return Orientation.SE;
          }
          if (direction.Dx < 0 && direction.Dy > 0) {
            return Orientation.NW;
          }
          if (direction.Dx < 0 && direction.Dy < 0) {
            return Orientation.SW;
          }
        }
        if (angle.IsBetween(bracketPI3, bracketPI2)) {
          if (direction.Dy > 0) {
            return Orientation.N;
          } else {
            return Orientation.S;
          }
        }
      } else {
        if (direction.Dy > 0) {
          return Orientation.N;
        } else {
          return Orientation.S;
        }
      }

      return null;
    }
  }
}
