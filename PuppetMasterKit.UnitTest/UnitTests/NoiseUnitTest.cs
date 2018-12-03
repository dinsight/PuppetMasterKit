
using System;
using CoreGraphics;
using NUnit.Framework;
using PuppetMasterKit.AI;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Ios.Isometric.Tilemap;
using PuppetMasterKit.Template.Game.Ios.Bindings;
using PuppetMasterKit.Terrain.Map;
using PuppetMasterKit.Terrain.Noise;
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
    public void GenerateFromTile(){
      var region = new Region(0);
      region.AddTile(0, 0);
      region.AddTile(0, 1);
      region.AddTile(1, 0);
      region.AddTile(1, 1);
      region.AddTile(2, 0);
      region.AddTile(2, 1);
      region.AddTile(2, 2);
      region.AddTile(3, 1);
      region.AddTile(3, 2);
      region.AddTile(3, 3);
      region.AddTile(4, 1);
      region.AddTile(4, 2);
      region.AddTile(4, 3);
      region.AddTile(3, 0);
      region.AddTile(4, 0);

      region.AddTile(0, 2);
      region.AddTile(0, 3);

      var tileSize = 128;
      var width = (region.MaxCol - region.MinCol+1) * tileSize + tileSize;
      var height = (region.MaxRow - region.MinRow+1) * tileSize;
      var data = GenerateFromRegion(region, tileSize);
      ImageFromBytes(data, width, height, $"/Users/alexjecu/Desktop/Workspace/dinsight/xamarin/assets/region.png");
    }


    /// <summary>
    /// Generates from region.
    /// </summary>
    /// <returns>The from region.</returns>
    /// <param name="region">Region.</param>
    /// <param name="tileSize">Tile size.</param>
    public byte[] GenerateFromRegion(Region region, int tileSize)
    {
      var im = new IsometricMapper(null);
      var width = (region.MaxCol - region.MinCol + 1) * tileSize + tileSize;
      var height = (region.MaxRow - region.MinRow + 1) * tileSize;

      var bytes = new byte[width * height * bytesPerPixel];
      var dim = 10;
      var gradient = GenerateWaterGradient(dim);
      var gradient2 = GeneratePerlinGradient(dim * 5);
      var depth = new Bicubic(gradient);
      var perlin = new Perlin(gradient2);
      var s = new int[] { 0x0, 0x0, 0xff, 0xff };
      var e = new int[] { 0xAF, 0xFF, 0xFF, 0xee };

      foreach (var tile in region.Tiles) {
        var xstart = tile.Col * tileSize;
        var ystart = tile.Row * tileSize;
        for (int j = ystart; j < ystart + tileSize; j++) {
          for (int i = xstart; i < xstart + tileSize; i++) {
            var py = j * (dim - 1f) / height;
            var px = i * (dim - 1f) / width;
            var iso = im.ToScene(new Point(i, j));
            var ii = (int)iso.X + width / 2;
            var ij = -(int)iso.Y;
            var noise = depth.Noise(px, py) +
                              perlin.Noise(px, py);
            noise = noise < -1 ? -1 : (noise > 1 ? 1 : noise);
            var index = (ij * width + ii) * bytesPerPixel;
            bytes[index] = (byte)(e[0] - (e[0] - s[0]) * (1 - noise) / 2);
            bytes[index + 1] = (byte)(e[1] - (e[1] - s[1]) * (1 - noise) / 2);
            bytes[index + 2] = (byte)(e[2] - (e[2] - s[2]) * (1 - noise) / 2);
            //bytes[index + 3] = (byte)(e[3] - (e[3] - s[3]) * (1 - noise) / 2);
            bytes[index + 3] = 0xFF;
          }
        }
      }
      return bytes;
    }

    /// <summary>
    /// Images from bytes.
    /// </summary>
    /// <param name="bytes">Bytes.</param>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    /// <param name="outputFile">Output file.</param>
    private void ImageFromBytes(byte[] bytes, int width, int height, String outputFile){
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

          image.SaveImage(outputFile);
        }
      }
    }

    /// <summary>
    /// Generates the gradient.
    /// </summary>
    /// <returns>The gradient.</returns>
    /// <param name="dim">Dim.</param>
    float[,] GenerateWaterGradient(int dim)
    {
      var gradient = new float[dim,dim];
      for (int i = 0; i < dim; i++) {
        for (int j = 0; j < dim; j++) {
          gradient[i,j] = random.Next(-100, 100) / 256f;
        }
      }

      for (int i = 0; i < dim; i++) {
        gradient[0,i] = 1;
        gradient[dim - 1,i] = 1;
        gradient[i,0] = 1;
        gradient[i,dim - 1] = 1;
      }

      return gradient;
    }

    float[,] GeneratePerlinGradient(int dim)
    {
      var gradient = new float[dim,dim];
      for (int i = 0; i < dim; i++) {
        for (int j = 0; j < dim; j++) {
          gradient[i,j] = random.Next(-255, 255) / 256f;
        }
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
