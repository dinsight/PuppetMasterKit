using System;
using CoreGraphics;
using NUnit.Framework;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Ios.Tiles.Tilemap.Helpers;
using PuppetMasterKit.Template.Game.Ios.Bindings;
using SpriteKit;

namespace PuppetMasterKit.UnitTest.UnitTests
{
  [TestFixture]
  public class HeightmapTest
  {
    const byte bitsPerComponent = 8;
    const byte bytesPerPixel = 4;
    const uint mask = (uint)CGImageAlphaInfo.PremultipliedLast | (uint)CGBitmapFlags.ByteOrder32Big;

    [Test]
    public void GenerateFromTile() {
      var tileSz = 128;
      var w = tileSz * 15;
      var h = tileSz * 15;
      var img = CreateMap(w, h, tileSz);
      img.SaveImage($"/Users/alexjecu/Desktop/Workspace/dinsight/xamarin/assets/map/perlin.png");
    }

    private CGImage CreateMap(int width, int height, int tileSize)
    {
      var im = new IsometricMapper(null);
      
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
              var ii = (int)iso.X + width / 2 - tileSize / 2;
              var ij = (int)iso.Y + height - tileSize / 2;
              var rect = new CGRect(ii, ij, tileSize, tileSize / 2);
              
            }
          }

          return context.ToImage();
        }
      }
    }
  }
}
