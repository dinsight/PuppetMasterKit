using System;
namespace PuppetMasterKit.Utility
{
  public class Color
  {
    public float Red { get; set; }
    public float Green { get; set; }
    public float Blue { get; set; }
    public float Alpha { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Utility.Color"/> class.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    /// <param name="alpha">Alpha.</param>
    public Color(float r, float g, float b, float alpha)
    {
      this.Red = r;
      this.Green = g;
      this.Blue = b;
      this.Alpha = alpha;
    }
  }
}
