using System;
using LightInject;
using System.Runtime.InteropServices;
using CoreGraphics;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;
using UIKit;
using System.Collections.Generic;

namespace PuppetMasterKit.Ios.Isometric.Tilemap
{
  public class TileHelper
  {
    /// <summary>
    /// Split.
    /// </summary>
    public class Split
    {
      public SKTexture topTile { get; set; }
      public SKTexture bottomTile { get; set; }
    }

    const byte bitsPerComponent = 8;
    const byte bytesPerPixel = 4;
    const uint mask = (uint)CGImageAlphaInfo.PremultipliedLast | (uint)CGBitmapFlags.ByteOrder32Big;

    /// <summary>
    /// Gets the bytes from image.
    /// </summary>
    /// <returns>The bytes from image.</returns>
    private  byte[] GetBytesFromImage(SKTexture texture)
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
    private CGImage GetImageFromBytes(SKTexture originalTexture, byte[] bytes)
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
    /// Gets the image from bytes.
    /// </summary>
    /// <returns>The image from bytes.</returns>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    /// <param name="bytes">Bytes.</param>
    private CGImage GetImageFromBytes(int width, int height, byte[] bytes)
    {
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

    /// <summary>
    /// Splits the tile.
    /// </summary>
    /// <returns>The tile.</returns>
    /// <param name="texture">Texture.</param>
    /// <param name="tileWidth">Tile width.</param>
    /// <param name="tileHeight">Tile height.</param>
    public Split SplitTile(SKTexture texture, int tileWidth, int tileHeight)
    {
      var split = new Split();
      var bytes = GetBytesFromImage(texture);
      var h = texture.CGImage.Height;
      var w = texture.CGImage.Width;
      var bpr = texture.CGImage.BytesPerRow;
      var bpp = (int)texture.CGImage.BitsPerPixel / bitsPerComponent;

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

    /// <summary>
    /// Ises the top tile.
    /// </summary>
    /// <returns><c>true</c>, if top tile was ised, <c>false</c> otherwise.</returns>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    /// <param name="tileWidth">Tile width.</param>
    /// <param name="tileHeight">Tile height.</param>
    private bool IsTopTile(int row, int col, int tileWidth, int tileHeight)
    {
      return row < tileHeight / 2;
    }

    /// <summary>
    /// Maps to image.
    /// </summary>
    /// <returns>The to image.</returns>
    /// <param name="node">Node.</param>
    /// <param name="tileSize">Tile size.</param>
    /// <param name="rows">Rows.</param>
    /// <param name="cols">Cols.</param>
    public CGImage FlattenNode(SKNode node, int tileSize, int rows, int cols)
    {
      CGImage image = null;
      var width = cols * tileSize;
      var height = rows * (tileSize / 2);
      using (var colourSpace = CGColorSpace.CreateDeviceRGB()) {
        using (var context = new CGBitmapContext(null,
                                                 width,
                                                 height,
                                                 bitsPerComponent,
                                                 0,
                                                 colourSpace,
                                                 (CGImageAlphaInfo)mask)) {

          foreach (SKSpriteNode item in node.Children) {
            context.DrawImage(new CGRect(item.Position.X + width / 2 - tileSize / 2, 
                                         height + item.Position.Y,
                                         item.Size.Width, 
                                         item.Size.Height), item.Texture.CGImage);
          }
          image = context.ToImage();
        }
      }
      return image;
    }

    /// <summary>
    /// Splits the image.
    /// </summary>
    /// <returns>The image.</returns>
    /// <param name="image">Image.</param>
    /// <param name="maxWidth">Max width.</param>
    /// <param name="maxHeight">Max height.</param>
    public SKSpriteNode SplitImage(CGImage image, int maxWidth, int maxHeight)
    {
      var imageWidth = (int)image.Width;
      var imageHeight = (int)image.Height;
      var sliceHCount = imageWidth % maxWidth != 0 ? 
                             imageWidth / maxWidth + 1 : 
                             imageWidth / maxWidth;
      var sliceVCount = imageHeight % maxHeight != 0 ? 
                             imageHeight / maxHeight + 1 : 
                             imageHeight / maxHeight;
      var sliceWidth = imageWidth / sliceHCount;
      var sliceHeight = imageHeight / sliceVCount;

      if(sliceHCount == 1 && sliceVCount == 1){
        return SKSpriteNode.FromTexture(SKTexture.FromImage(image));
      }

      var parentNode = new SKSpriteNode();        
      for (int v = 0; v < sliceVCount; v++) {
        for (int h = 0; h < sliceHCount; h++) {
          var rowStart = v * sliceHeight;
          var rowEnd = (int)Math.Min(rowStart + sliceHeight, image.Height);
          var colStart = h * sliceWidth;
          var colEnd = (int)Math.Min(colStart + sliceWidth, image.Width);
          var slice = CopyFromImage(image, rowStart, rowEnd, colStart, colEnd);
          var node = SKSpriteNode.FromTexture(SKTexture.FromImage(slice));
          parentNode.AddChild(node);
          node.AnchorPoint = new CGPoint(0, 1);
          node.Position = new CGPoint(h*sliceWidth - imageWidth/2, -v*sliceHeight);
        }
      }
      return parentNode;
    }

    /// <summary>
    /// Copies from image.
    /// </summary>
    /// <returns>The from image.</returns>
    /// <param name="image">Image.</param>
    /// <param name="rowStart">Row start.</param>
    /// <param name="rowEnd">Row end.</param>
    /// <param name="colStart">Col start.</param>
    /// <param name="colEnd">Col end.</param>
    private CGImage CopyFromImage(CGImage image, int rowStart, int rowEnd, int colStart, int colEnd)
    {
      var width = colEnd - colStart;
      var height = rowEnd - rowStart;


      using (var data = image.DataProvider.CopyData()) { 
        var imageBuffer = new byte[data.Length];
        Marshal.Copy(data.Bytes, imageBuffer, 0, (int)data.Length);

        var bytes = new byte[width * height * bytesPerPixel];
        for (int row = rowStart; row < rowEnd; row++) {
          for (int col = colStart; col < colEnd; col++) {
            var sourceIndex = (row * image.Width + col) * bytesPerPixel;
            var destIndex = ((row - rowStart) * width + col - colStart) * bytesPerPixel;
            for (int b = 0; b < bytesPerPixel; b++) {
              bytes[destIndex + b] = imageBuffer[sourceIndex + b];
            }
          }
        }
        return GetImageFromBytes(width, height, bytes);
      }
    }

    /// <summary>
    /// Saves the image.
    /// </summary>
    /// <param name="image">Image.</param>
    /// <param name="fileName">File name.</param>
    public void SaveImage(CGImage image, string fileName)
    {
      UIImage img = UIImage.FromImage(image);
      using (var data = img.AsPNG()) {
        data.Save(fileName, false);
      }
    }
  }
}
