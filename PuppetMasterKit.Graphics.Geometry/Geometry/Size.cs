using System;
namespace PuppetMasterKit.Graphics.Geometry
{
    public class Size
    {
        public static Size Zero = new Size();
        
        public float Width { get; set; }

        public float Height { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Graphics.Geometry.Size"/> class.
        /// </summary>
        public Size()
        {
            Width = 0;
            Height = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Graphics.Geometry.Size"/> class.
        /// </summary>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public Size(float width, float height)
        {
            this.Width = width;
            this.Height = height;
        }
    }
}
