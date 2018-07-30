using System;
using System.Runtime.InteropServices;
using CoreGraphics;
using SpriteKit;

namespace PuppetMasterKit.Tilemap
{
  public static class TileHelper
  {
    public class Split{
      public SKTexture topTile { get; set; }
      public SKTexture bottomTile { get; set; }
    }
    /// <summary>
    /// Gets the bytes from image.
    /// </summary>
    /// <returns>The bytes from image.</returns>
    private static byte[] GetBytesFromImage(SKTexture texture)
    {
      using (var data = texture.CGImage.DataProvider.CopyData()) {
        var bytes = new byte[data.Length];
        Marshal.Copy(data.Bytes, bytes, 0, (int)data.Length);
        return bytes;
      }
    }

    /// <summary>
    /// Gets the image from bytes.
    /// </summary>
    /// <returns>The image from bytes.</returns>
    /// <param name="originalTexture">Original texture.</param>
    /// <param name="bytes">Bytes.</param>
    private static CGImage GetImageFromBytes(SKTexture originalTexture, byte[] bytes)
    {
      CGImage cgImage = null;
      using (var dataProvider = new CGDataProvider(bytes, 0, bytes.Length)) {
        using (var colourSpace = CGColorSpace.CreateDeviceRGB()) {
          cgImage = new CGImage((int)originalTexture.CGImage.Width, 
                                (int)originalTexture.CGImage.Height,
                                (int)originalTexture.CGImage.BitsPerComponent,
                                (int)originalTexture.CGImage.BitsPerPixel,
                                (int)originalTexture.CGImage.BytesPerRow,
                                colourSpace,
                                originalTexture.CGImage.BitmapInfo,
                                dataProvider,
                                null,
                                originalTexture.CGImage.ShouldInterpolate,
                                originalTexture.CGImage.RenderingIntent);
        }
      }
      return cgImage;
    }

    /// <summary>
    /// Splits the tile.
    /// </summary>
    /// <returns>The tile.</returns>
    /// <param name="texture">Texture.</param>
    /// <param name="tileWidth">Tile width.</param>
    /// <param name="tileHeight">Tile height.</param>
    public static Split SplitTile(SKTexture texture, int tileWidth, int tileHeight)
    {
      var split = new Split();
      var bytes = GetBytesFromImage(texture);
      var h = texture.CGImage.Height;
      var w = texture.CGImage.Width;
      var bpr = texture.CGImage.BytesPerRow;
      var bpp = (int)texture.CGImage.BitsPerPixel / 8;

      var topBufferSize = (h - tileHeight/2) * w * bpr;
      var bottomBufferSize = tileHeight * w * bpp;
      var topBuffer = new byte[topBufferSize];
      var bottomBuffer = new byte[bottomBufferSize];
      var bottomStartIndex = bytes.Length - bottomBufferSize;

      for (int index = 0; index < bytes.Length; index += bpp) {
        var row = (index / bpp) / w;
        var col = index - (w * row);
        if(IsTopTile((int)row, (int)col, tileWidth, tileHeight)){
          topBuffer[index] = bytes[index];
          topBuffer[index + 1] = bytes[index + 1];
          topBuffer[index + 2] = bytes[index + 2];
          topBuffer[index + 3] = bytes[index + 3];
        }else{
          var bottomIndex = index - bottomStartIndex;
          bottomBuffer[bottomIndex] = bytes[index];
          bottomBuffer[bottomIndex + 1] = bytes[index + 1];
          bottomBuffer[bottomIndex + 2] = bytes[index + 2];
          bottomBuffer[bottomIndex + 3] = bytes[index + 3];
        }
      }

      var imageTop = GetImageFromBytes(texture, topBuffer);
      var imageBottom = GetImageFromBytes(texture, bottomBuffer);
      split.topTile = SKTexture.FromImage(imageTop);
      split.bottomTile = SKTexture.FromImage(imageBottom);

      return split;
    }

    private static bool IsTopTile(int row, int col, int tileWidth, int tileHeight)
    {
      return row < tileHeight / 2;
    }
  }
}
