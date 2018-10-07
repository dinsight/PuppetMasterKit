
using System;
using CoreGraphics;
using CoreImage;
using NUnit.Framework;
using PuppetMasterKit.AI;
using PuppetMasterKit.Graphics;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Ios.Isometric.Tilemap;
using PuppetMasterKit.Template.Game.Ios.Bindings;
using SpriteKit;

namespace PuppetMasterKit.UnitTest.UnitTests
{
  [TestFixture]
  public class NoiseUnitTest
  {
    Random random = new Random(Guid.NewGuid().GetHashCode());

    const byte bitsPerComponent = 8;
    const byte bytesPerPixel = 4;
    const uint mask = (uint)CGImageAlphaInfo.PremultipliedLast | (uint)CGBitmapFlags.ByteOrder32Big;



    [Test]
    public void GenerateDepth()
    {
      var im = new IsometricMapper(null);
      int width = 1280;
      int height = 1280;
      var bytes = new byte[width * height * bytesPerPixel];
      var dim = 10;
      var gradient = GenerateGradient(dim);
      var depth = new Bicubic(gradient);
      var s = new int[] { 0x0, 0x0, 0xff };
      var e = new int[] { 0xFF, 0xFF, 0xFF };

      for (int j = 0; j < height; j++) {
        for (int i = 0; i < width; i++) {
          var iso = im.ToScene(new Point(i, j));
          var ii = (int)iso.X + width/2;
          var ij = -(int)iso.Y;
          //var ii = i;
          //var ij = j;
          var index = (ij * width + ii)*bytesPerPixel;
          var py = j * (dim - 1f) / height;
          var px = i * (dim - 1f) / width;
          var noise = depth.Noise(px, py);
          noise = noise < -1 ? -1: (noise > 1 ? 1 : noise);
          bytes[index]   = (byte)(e[0] - (e[0] - s[0]) * (1 - noise) / 2);
          bytes[index+1] = (byte)(e[1] - (e[1] - s[1]) * (1 - noise) / 2);
          bytes[index+2] = (byte)(e[2] - (e[2] - s[2]) * (1 - noise) / 2);
          bytes[index+3] = 0xFF;
          //bytes[index] = (byte)e[0];
          //bytes[index+1] = (byte)e[1];
          //bytes[index+2] = (byte)e[2];
          //bytes[index+3] = (byte)(0xff * (1 + noise) / 2);
        }
      }

      CGImage image;
      using (var colourSpace = CGColorSpace.CreateDeviceRGB()) {
        using (var context = new CGBitmapContext(bytes,
                                                 width,
                                                 height,
                                                 bitsPerComponent,
                                                 width * bytesPerPixel,
                                                 colourSpace,
                                                 (CGImageAlphaInfo)mask)) {
          image = context.ToImage();

          image.SaveImage($"/Users/alexjecu/Desktop/Workspace/dinsight/xamarin/assets/perlin.png");
        }
      }

      //var water = CreateWater(width, height, 128);
      //var shore = CreateShore(width, height, 120);
      //shore.SaveImage($"/Users/alexjecu/Desktop/Workspace/dinsight/xamarin/assets/shore.png");
      //// Create a CIBlendWithAlphaMask filter with our three input images 
      //var blend_with_alpha_mask = new CIBlendWithAlphaMask() {
      //  BackgroundImage = water,
      //  Image = shore,
      //  Mask = image
      //};
      //var output = blend_with_alpha_mask.OutputImage;
      //var ctx = CIContext.FromOptions(null);
      //var cgimage = ctx.CreateCGImage(output, output.Extent);
      //cgimage.SaveImage($"/Users/alexjecu/Desktop/Workspace/dinsight/xamarin/assets/blended.png");
    }

    /// <summary>
    /// Generates the gradient.
    /// </summary>
    /// <returns>The gradient.</returns>
    /// <param name="dim">Dim.</param>
    float[][] GenerateGradient(int dim)
    {
      var gradient = new float[dim][];
      for (int i = 0; i < dim; i++) {
        gradient[i] = new float[dim];
        for (int j = 0; j < dim; j++) {
          gradient[i][j] = random.Next(-255, 255) / 256f;
        }
      }

      for (int i = 0; i < dim; i++) {
        gradient[0][i] = 1;
        gradient[dim - 1][i] = 1;
        gradient[i][0] = 1;
        gradient[i][dim - 1] = 1;
      }

      return gradient;
    }

    /// <summary>
    /// Creates the water.
    /// </summary>
    /// <returns>The water.</returns>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    /// <param name="tileSize">Tile size.</param>
    private CGImage CreateWater(int width, int height, int tileSize)
    {
      var im = new IsometricMapper(null);
      var background = SKTexture.FromImageNamed("Water").CGImage;
      using (var colourSpace = CGColorSpace.CreateDeviceRGB()) {
        using (var context = new CGBitmapContext(null,
                                                 width,
                                                 height,
                                                 bitsPerComponent,
                                                 width * bytesPerPixel,
                                                 colourSpace,
                                                 (CGImageAlphaInfo)mask)) {

          for (int j = 0; j < height; j += tileSize) {
            for (int i = 0; i < width; i += tileSize) {
              var iso = im.ToScene(new Point(i, j));
              var ii = (int)iso.X + width / 2 - tileSize/2;
              var ij = (int)iso.Y + height - tileSize/2;
              var rect = new CGRect(ii, ij, tileSize, tileSize / 2);
              context.DrawImage(rect, background);
            }
          }

          return context.ToImage();
        }
      }
    }

    private CGImage CreateShore(int width, int height, int tileSize)
    {
      var im = new IsometricMapper(null);
      var bytes = new byte[width * height * bytesPerPixel];
      for (int j = 0; j < height; j++) {
        for (int i = 0; i < width; i++) {
          var iso = im.ToScene(new Point(i, j));
          var ii = (int)iso.X + width / 2;
          var ij = -(int)iso.Y;
          //var ii = i;
          //var ij = j;
          var index = (ij * width + ii) * bytesPerPixel;

          bytes[index] = 0xc1;
          bytes[index + 1] = 0xfb;
          bytes[index + 2] = 0xff;
          bytes[index + 3] = 0xff;
        }
      }
      using (var colourSpace = CGColorSpace.CreateDeviceRGB()) {
        using (var context = new CGBitmapContext(bytes,
                                                 width,
                                                 height,
                                                 bitsPerComponent,
                                                 width * bytesPerPixel,
                                                 colourSpace,
                                                 (CGImageAlphaInfo)mask)) {

          return context.ToImage();
        }
      }
    }
  }
}
