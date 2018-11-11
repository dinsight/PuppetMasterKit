using System;
using CoreGraphics;
using LightInject;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Terrain.Noise;
using SpriteKit;

namespace PuppetMasterKit.Ios.Isometric.Tilemap
{
  public class TilePainter
  {
    private readonly int tileSize;
    private ICoordinateMapper mapper;
    private readonly byte[] data;
    private int row;
    private int col;
    private int imageWidth;
    private int imageHeight;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Ios.Isometric.Tilemap.TilePainter"/> class.
    /// </summary>
    /// <param name="tileSize">Tile size.</param>
    /// <param name="imageWidth">Image width.</param>
    /// <param name="imageHeight">Image height.</param>
    /// <param name="imageData">Image data.</param>
    public TilePainter(int tileSize, int imageWidth, int imageHeight, byte[] imageData)
    {
      this.tileSize = tileSize;
      this.mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
      this.data = imageData;
      this.imageWidth = imageWidth;
      this.imageHeight = imageHeight;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Ios.Isometric.Tilemap.TilePainter"/> class.
    /// </summary>
    /// <param name="tileSize">Tile size.</param>
    /// <param name="imageWidth">Image width.</param>
    /// <param name="imageHeight">Image height.</param>
    public TilePainter(int tileSize, int imageWidth, int imageHeight)
    {
      this.tileSize = tileSize;
      this.mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
      this.data = new byte[tileSize * (tileSize / 2) * ImageHelper.BytesPerPixel]; ;
      this.imageWidth = imageWidth;
      this.imageHeight = imageHeight;
    }

    /// <summary>
    /// Sets the coords.
    /// </summary>
    /// <returns>The coords.</returns>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    public TilePainter SetTileContext(int row, int col)
    {
      this.row = row;
      this.col = col;
      return this;
    }

    /// <summary>
    /// Convert to texture.
    /// </summary>
    /// <returns>The texture.</returns>
    public SKTexture ToTexture(){
      var image = ImageHelper.GetImageFromBytes(tileSize, tileSize / 2, data);
      //create texture from mage
      var texture = SKTexture.FromImage(image);
      return texture;
    }

    /// <summary>
    /// Paints the left side alpha.
    /// </summary>
    public TilePainter PaintLeftSideAlpha() => PaintSideAlpha((x, y) => Point.Distance(tileSize, 0, x, 0));
    /// <summary>
    /// Paints the right side alpha.
    /// </summary>
    public TilePainter PaintRightSideAlpha() => PaintSideAlpha((x, y) => Point.Distance(0, 0, x, 0));
    /// <summary>
    /// Paints the top side alpha.
    /// </summary>
    public TilePainter PaintTopSideAlpha() => PaintSideAlpha((x, y) => Point.Distance(0, 0, 0, y));
    /// <summary>
    /// Paints the bottom side alpha.
    /// </summary>
    public TilePainter PaintBottomSideAlpha() => PaintSideAlpha((x, y) => Point.Distance(0, tileSize, 0, y));
    /// <summary>
    /// Paints the top left joint alpha.
    /// </summary>
    public TilePainter PaintTopLeftJointAlpha() => PaintJointAlpha((x, y) => Point.Distance(0, tileSize, x, y));
    /// <summary>
    /// Paints the top right joint alpha.
    /// </summary>
    public TilePainter PaintTopRightJointAlpha() => PaintJointAlpha((x, y) => Point.Distance(tileSize, tileSize, x, y));
    /// <summary>
    /// Paints the bottom left joint alpha.
    /// </summary>
    public TilePainter PaintBottomLeftJointAlpha() => PaintJointAlpha((x, y) => Point.Distance(0, 0, x, y));
    /// <summary>
    /// Paints the bottom right joint alpha.
    /// </summary>
    public TilePainter PaintBottomRightJointAlpha() => PaintJointAlpha((x, y) => Point.Distance(tileSize, 0, x, y));
    /// <summary>
    /// Paints the top left corner alpha.
    /// </summary>
    /// <returns>The top left corner alpha.</returns>
    public TilePainter PaintTopLeftCornerAlpha() => PaintCornerAlpha(new Point(1, 0));
    /// <summary>
    /// Paints the top right corner alpha.
    /// </summary>
    /// <returns>The top right corner alpha.</returns>
    public TilePainter PaintTopRightCornerAlpha() => PaintCornerAlpha(new Point(0, 0));
    /// <summary>
    /// Paints the bottom left corner alpha.
    /// </summary>
    /// <returns>The bottom left corner alpha.</returns>
    public TilePainter PaintBottomLeftCornerAlpha() => PaintCornerAlpha(new Point(1, 1));
    /// <summary>
    /// Paints the bottom right corner alpha.
    /// </summary>
    /// <returns>The bottom right corner alpha.</returns>
    public TilePainter PaintBottomRightCornerAlpha() => PaintCornerAlpha(new Point(0,1));

    /// <summary>
    /// 2D to ISO to bitmap coordinate conversion
    /// </summary>
    /// <returns>The bitmap index.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    private int GetBitmapIndex(int x, int y)
    {
      var iso = mapper.ToScene(new Point(x, y));
      //set the x=0 coord to be the middle of the bitmap
      var ii = (int)iso.X + imageWidth / 2;
      //the CGImage's origin is in the top-left corner of the bitmap
      //the x goes from left to right, and y grows top-down
      var ij = -(int)iso.Y;
      var index = (ij * imageWidth + ii) * ImageHelper.BytesPerPixel;
      return index;
    }

    /// <summary>
    /// Scans the pixels.
    /// </summary>
    /// <param name="action">Action.</param>
    private void ScanPixels(Action<int, int> action)
    {
      for (int y = 0; y < tileSize; y++) {
        for (int x = 0; x < tileSize; x++) {
          action(x, y);
        }
      }
    }

    /// <summary>
    /// Paints a joint at the (row, col) tile coordinates
    /// </summary>
    /// <returns>The joint alpha.</returns>
    /// <param name="distFn">Dist fn.</param>
    private TilePainter PaintJointAlpha(Func<float, float, float> distFn)
    {
      var r = tileSize/2;
      var d = tileSize/2;

      ScanPixels((x, y) => {
        int index = GetBitmapIndex(x, y);
        var dist = distFn(x - col * tileSize, y - row * tileSize);
        //var dist = distFn(x, y);
        var c = (dist - r) > r ? 0xff : 0xff * (dist - r) / d;
        var alpha = dist >= r ? c : 0x0;
        data[index + 3] = (byte)alpha;
      });
      return this;
    }

    /// <summary>
    /// Paints a side at the row,col tile coordinates
    /// </summary>
    /// <returns>The side alpha.</returns>
    /// <param name="distFn">Dist fn.</param>
    private TilePainter PaintSideAlpha( Func<float, float, float> distFn)
    {
      var r = tileSize / 2;
      ScanPixels((x, y) => {
        int index = GetBitmapIndex(x, y);
        var dist = distFn(x-col*tileSize, y-row*tileSize);
        var c = 0xff * (r - dist) / r;
        var alpha = dist < r ? c : 0x0;
        data[index + 3] = (byte)alpha;
      });
      return this;
    }

    /// <summary>
    /// Paints a corner at the row,col tile coordinates
    /// </summary>
    /// <returns>The corner alpha.</returns>
    /// <param name="point">Point.</param>
    private TilePainter PaintCornerAlpha(Point point)
    {
      var r = 0.5;
      ScanPixels((x, y) => {
        var dist = Point.Distance(point.X, point.Y, (float)x/tileSize, (float)y/tileSize);
        int index = GetBitmapIndex(x, y);
        var f = (r - dist) / r;
        var c = 0xff * (r - dist) / r;
        var alpha = dist < r ? c : 0x0;
        data[index + 3] = (byte)alpha;
      });
      return this;
    }

    /// <summary>
    /// Renders noise at the row, col tile coordinates
    /// </summary>
    /// <returns>The noise.</returns>
    /// <param name="noise">Noise.</param>
    /// <param name="scaleX">Scale x.</param>
    /// <param name="scaleY">Scale y.</param>
    /// <param name="startColor">Start color.</param>
    /// <param name="endColor">End color.</param>
    public TilePainter PaintNoise( INoiseGenerator noise, 
                                   float scaleX, 
                                   float scaleY, 
                                   int[] startColor, 
                                   int[] endColor  )
    {

      for (int y = 0; y < tileSize; y++) {
        for (int x = 0; x < tileSize; x++) {
          int index = GetBitmapIndex(x, y);
          //the niose's coordinates are on the top left
          //the tile grid's coord are bottom left
          var px = (x + row * tileSize) * scaleY;
          var py = (y + col * tileSize) * scaleX;
          var value = noise.Noise(px, py);
          value = value < -1 ? -1 : (value > 1 ? 1 : value);
          data[index] = (byte)(endColor[0] - (endColor[0] - startColor[0]) * (1 - value) / 2);
          data[index + 1] = (byte)(endColor[1] - (endColor[1] - startColor[1]) * (1 - value) / 2);
          data[index + 2] = (byte)(endColor[2] - (endColor[2] - startColor[2]) * (1 - value) / 2);
          //bytes[index + 3] = (byte)(e[3] - (e[3] - s[3]) * (1 - noise) / 2);
          data[index + 3] = 0xFF;
        }
      }
      //ScanPixels((x,y)=>{
      //  int index = GetBitmapIndex(x, y);
      //  //the niose's coordinates are on the top left
      //  //the tile grid's coord are bottom left
      //  var px = (x + row * tileSize) * scaleY;
      //  var py = (y + col * tileSize) * scaleX;
      //  var value = noise.Noise(px, py);
      //  value = value < -1 ? -1 : (value > 1 ? 1 : value);
      //  data[index]     = (byte)(endColor[0] - (endColor[0] - startColor[0]) * (1 - value) / 2);
      //  data[index + 1] = (byte)(endColor[1] - (endColor[1] - startColor[1]) * (1 - value) / 2);
      //  data[index + 2] = (byte)(endColor[2] - (endColor[2] - startColor[2]) * (1 - value) / 2);
      //  //bytes[index + 3] = (byte)(e[3] - (e[3] - s[3]) * (1 - noise) / 2);
      //  data[index + 3] = 0xFF;
      //});
      return this;
    }

    public TilePainter PaintSolid(int[] color)
    {
      ScanPixels((x, y) => {
        int index = GetBitmapIndex(x, y);
        data[index] = (byte)color[0];
        data[index+1] = (byte)color[1];
        data[index+2] = (byte)color[2];
        data[index+3] = (byte)color[3];
      });
      return this;
    }
  }
}
