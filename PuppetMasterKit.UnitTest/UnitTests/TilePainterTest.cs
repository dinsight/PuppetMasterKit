using System;
using NUnit.Framework;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Ios.Isometric.Tilemap;
using PuppetMasterKit.Template.Game.Ios.Bindings;
using PuppetMasterKit.Terrain.Map;
using PuppetMasterKit.Terrain.Noise;

namespace PuppetMasterKit.UnitTest.UnitTests
{
  [TestFixture]
  public class TilePainterTest
  {
    internal int tileSize = 128;
    readonly string basePath = "/Users/alexjecu/Desktop/Workspace/dinsight/xamarin/assets";

    Random random = new Random(Guid.NewGuid().GetHashCode());


    [Test]
    public void PaintNoise()
    {
      int dim = 10;
      int size = tileSize * dim;

      var s = new int[] { 0x0, 0x0, 0xff, 0xff };
      var e = new int[] { 0xAF, 0xFF, 0xFF, 0xee };
      var buffer = new byte[size * (size / 2) * ImageHelper.BytesPerPixel];
      Container.GetContainer().Register<ICoordinateMapper>(factory => new IsometricMapper(null));
      var gradient = GenerateGradient(dim);
      var depth = new Bicubic(gradient);
      var painter = new TilePainter(tileSize, size, size, buffer);
      var scale = (float)dim / size;

      var region = new Region(0);
      //region.AddTile(0, 1);
      //region.AddTile(1, 0);
      //region.AddTile(1, 1);

      region.AddTile(3, 2);
      region.AddTile(4, 1);
      region.AddTile(4, 2);
      region.AddTile(4, 3);
      region.AddTile(5, 1);
      region.AddTile(5, 2);
      region.AddTile(5, 3);
      region.AddTile(6, 1);
      region.AddTile(6, 2);
      region.AddTile(6, 3);
      region.AddTile(7, 2);


      foreach (var item in region.Tiles) {
        painter.SetTileContext(item.Row, item.Col)
             .PaintNoise(depth, scale, scale, s, e);
      }
      /*
      painter.SetTileContext(3, 1).PaintNoise(depth, scale, scale, s, e).PaintBottomLeftJointAlpha();
      painter.SetTileContext(3, 3).PaintNoise(depth, scale, scale, s, e).PaintBottomRightJointAlpha();
      painter.SetTileContext(7, 3).PaintNoise(depth, scale, scale, s, e).PaintTopRightJointAlpha();
      painter.SetTileContext(7, 1).PaintNoise(depth, scale, scale, s, e).PaintTopLeftJointAlpha();

      painter.SetTileContext(4, 4).PaintNoise(depth, scale, scale, s, e).PaintRightSideAlpha();
      painter.SetTileContext(5, 4).PaintNoise(depth, scale, scale, s, e).PaintRightSideAlpha();
      painter.SetTileContext(6, 4).PaintNoise(depth, scale, scale, s, e).PaintRightSideAlpha();

      painter.SetTileContext(6, 0).PaintNoise(depth, scale, scale, s, e).PaintLeftSideAlpha();
      painter.SetTileContext(5, 0).PaintNoise(depth, scale, scale, s, e).PaintLeftSideAlpha();
      painter.SetTileContext(4, 0).PaintNoise(depth, scale, scale, s, e).PaintLeftSideAlpha();

      painter.SetTileContext(8, 2).PaintNoise(depth, scale, scale, s, e).PaintTopSideAlpha();
      painter.SetTileContext(2, 2).PaintNoise(depth, scale, scale, s, e).PaintBottomSideAlpha();

      painter.SetTileContext(3, 4).PaintNoise(depth, scale, scale, s, e).PaintBottomRightCornerAlpha();
      painter.SetTileContext(2, 3).PaintNoise(depth, scale, scale, s, e).PaintBottomRightCornerAlpha();
      painter.SetTileContext(7, 4).PaintNoise(depth, scale, scale, s, e).PaintTopRightCornerAlpha();
      painter.SetTileContext(8, 3).PaintNoise(depth, scale, scale, s, e).PaintTopRightCornerAlpha();
      painter.SetTileContext(8, 1).PaintNoise(depth, scale, scale, s, e).PaintTopLeftCornerAlpha();
      painter.SetTileContext(7, 0).PaintNoise(depth, scale, scale, s, e).PaintTopLeftCornerAlpha();
      painter.SetTileContext(3, 0).PaintNoise(depth, scale, scale, s, e).PaintBottomLeftCornerAlpha();
      painter.SetTileContext(2, 1).PaintNoise(depth, scale, scale, s, e).PaintBottomLeftCornerAlpha();
*/
      ImageHelper.GetImageFromBytes(size, size/2, buffer)
                 .SaveImage($"{basePath}/noise.png");
    }

    [Test]
    public void PaintCorners()
    {
      Container.GetContainer().Register<ICoordinateMapper>(factory => new IsometricMapper(null));
      var painter = new TilePainter(tileSize, tileSize, tileSize / 2);

      painter.SetTileContext(0, 0)
             .PaintBottomLeftJointAlpha()
             .ToTexture().CGImage.SaveImage($"{basePath}/map/blj.png");

      painter.SetTileContext(0, 0)
             .PaintBottomRightJointAlpha()
             .ToTexture().CGImage.SaveImage($"{basePath}/map/brj.png");

      painter.SetTileContext(0, 0)
             .PaintTopRightJointAlpha()
             .ToTexture().CGImage.SaveImage($"{basePath}/map/trj.png");

      painter.SetTileContext(0, 0)
             .PaintTopLeftJointAlpha()
             .ToTexture().CGImage.SaveImage($"{basePath}/map/tlj.png");
        
      painter.SetTileContext(0, 0)
             .PaintRightSideAlpha()
             .ToTexture().CGImage.SaveImage($"{basePath}/map/rs.png");

      painter.SetTileContext(0, 0)
             .PaintLeftSideAlpha()
             .ToTexture().CGImage.SaveImage($"{basePath}/map/ls.png");

      painter.SetTileContext(0, 0)
             .PaintTopSideAlpha()
             .ToTexture().CGImage.SaveImage($"{basePath}/map/ts.png");

      painter.SetTileContext(0, 0)
             .PaintBottomSideAlpha()
             .ToTexture().CGImage.SaveImage($"{basePath}/map/bs.png");

      painter.SetTileContext(0, 0)
             .PaintBottomRightCornerAlpha()
             .ToTexture().CGImage.SaveImage($"{basePath}/map/brc.png");

      painter.SetTileContext(0, 0)
             .PaintTopRightCornerAlpha()
             .ToTexture().CGImage.SaveImage($"{basePath}/map/trc.png");

      painter.SetTileContext(0, 0)
             .PaintBottomRightCornerAlpha()
             .ToTexture().CGImage.SaveImage($"{basePath}/map/tlc.png");

      painter.SetTileContext(0, 0)
             .PaintBottomLeftCornerAlpha()
             .ToTexture().CGImage.SaveImage($"{basePath}/map/blc.png");
    }

    /// <summary>
    /// Generates the gradient.
    /// </summary>
    /// <returns>The gradient.</returns>
    /// <param name="dim">Dim.</param>
    float[,] GenerateGradient(int dim)
    {
      var gradient = new float[dim,dim];
      for (int i = 0; i < dim; i++) {
        for (int j = 0; j < dim; j++) {
          gradient[i,j] = random.Next(-100, 100) / 256f;
        }
      }
      return gradient;
    }
  }
}
