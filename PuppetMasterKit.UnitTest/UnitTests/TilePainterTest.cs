using System;
using NUnit.Framework;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Noise;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Ios.Isometric.Tilemap;
using PuppetMasterKit.Template.Game.Ios.Bindings;

namespace PuppetMasterKit.UnitTest.UnitTests
{
  [TestFixture]
  public class TilePainterTest
  {
    internal int tileSize = 128;
    readonly string basePath = "/Users/alexjecu/Desktop/Workspace/dinsight/xamarin/assets";

    Random random = new Random(Guid.NewGuid().GetHashCode());

    [Test]
    public void BmpCoords()
    {
      var dim = 128;
      var data = new byte[dim * dim * ImageHelper.BytesPerPixel];

      for (int i = 0; i < dim; i++) {
        for (int j = 0; j < dim; j++) {
          var index = (i * dim + j) * ImageHelper.BytesPerPixel;
          if(i==0&&j==0){
            data[index] = 0xFF;
            data[index+1] = 0x00;
            data[index+2] = 0x00;
            data[index+3] = 0xFF;
          } else {
            data[index] = 0xFF;
            data[index + 1] = 0xFF;
            data[index + 2] = 0xFF;
            data[index + 3] = 0xFF;
          }
        }
      }

      ImageHelper.GetImageFromBytes(dim, dim, data).SaveImage($"{basePath}/orig.png");
    }


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

      //painter.SetTileContext(0, 0).PaintBottomLeftJointAlpha();

      ImageHelper.GetImageFromBytes(size, size/2, buffer)
                 .SaveImage($"{basePath}/noise.png");
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
          gradient[i][j] = random.Next(-100, 100) / 256f;
        }
      }

      //for (int i = 0; i < dim; i++) {
      //  gradient[0][i] = 1;
      //  gradient[dim - 1][i] = 1;
      //  gradient[i][0] = 1;
      //  gradient[i][dim - 1] = 1;
      //}

      return gradient;
    }
  }
}
