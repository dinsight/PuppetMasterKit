using System;
using System.Linq;
using System.Runtime.InteropServices;
using CoreImage;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;
using PuppetMasterKit.AI.Configuration;
using LightInject;
using PuppetMasterKit.Graphics.Geometry;
using System.Collections.Generic;

namespace PuppetMasterKit.Ios.Isometric
{
  public static class TextureHelper
  {

    static Random randomTexture = new Random(Guid.NewGuid().GetHashCode());

    static ICoordinateMapper mapper;

    static TextureHelper()
    {
      mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
    }
    /// <summary>
    /// Gets the texture.
    /// </summary>
    /// <returns>The texture.</returns>
    /// <param name="group">Group.</param>
    /// <param name="ruleName">Rule name.</param>
    public static SKTexture GetRandomTexture(this SKTileGroup group, string ruleName){
      if (group.Rules.Count() == 1) {
        return group.Rules
                    .First()
                    .TileDefinitions
                    .First()
                    .Textures
                    .First();
      }

      var tileDefs = group.Rules
                          .Where(r => r.Name == ruleName && r.TileDefinitions != null)
                          .SelectMany(x => x.TileDefinitions).ToList();
      if (tileDefs.Count == 0) {
        return null;
      }

      if (tileDefs.Count == 1) {
        return tileDefs.First().Textures.First();
      }

      var index = randomTexture.Next(0, tileDefs.Count());
      return tileDefs[index].Textures.First();
    }

    /// <summary>
    /// Gets the textures.
    /// </summary>
    /// <returns>The textures.</returns>
    /// <param name="group">Group.</param>
    /// <param name="ruleName">Rule name.</param>
    public static List<SKTexture> GetTextures(this SKTileGroup group, string ruleName) 
    {
      var tileDefs = group.Rules
                          .Where(r => r.Name == ruleName && r.TileDefinitions != null)
                          .SelectMany(x => x.TileDefinitions);

      return tileDefs.SelectMany(x => x.Textures).ToList();
    }

    /// <summary>
    /// Blends the with alpha.
    /// </summary>
    /// <returns>The with alpha.</returns>
    /// <param name="texture">Texture.</param>
    /// <param name="textureAlpha">Texture alpha.</param>
    public static SKTexture BlendWithAlpha(this SKTexture texture, SKTexture textureAlpha)
    {
      //hack - mono won't link if I do not create a CIBlendWithMask instance
      //It probably helps the linker figure out where the definiton is.
      //Also, the CoreImage namespace has to be explicitly specified.
      var tm = new CoreImage.CIBlendWithMask();
      var blend = new CoreImage.CIBlendWithAlphaMask() {
        BackgroundImage = null,
        Image = texture.CGImage,
        Mask = textureAlpha.CGImage
      };
      var output = blend.OutputImage;
      var context = CIContext.FromOptions(null);
      var cgimage = context.CreateCGImage(output, output.Extent);
      var blendedTexture = SKTexture.FromImage(cgimage);
      return blendedTexture;
      //return texture;
    }

    /// <summary>
    /// Gets the bytes from texture.
    /// </summary>
    /// <returns>The bytes from texture.</returns>
    /// <param name="texture">Texture.</param>
    public static byte[] GetBytesFromTexture(this SKTexture texture)
    {
      using (var data = texture.CGImage.DataProvider.CopyData()) {
        var bytes = new byte[data.Length];
        Marshal.Copy(data.Bytes, bytes, 0, (int)data.Length);
        return bytes;
      }
    }

    /// <summary>
    /// Sets the texture.
    /// </summary>
    /// <param name="texture">Texture.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public static void SetTexture(this SKTexture texture, SKNode dest, float x, float y, float? zPos = null)
    {

      if (texture != null) {
        var node = SKSpriteNode.FromTexture(texture);
        //BUG : anchor point is not working properly
        //We substract half the width so the image gets centered on the x axis
        //var scenePos = mapper.ToScene(new Point(x+(float)node.Size.Width/2, y + GetMap().TileSize/2));
        var scenePos = mapper.ToScene(new Point(x, y));
        node.AnchorPoint = new CoreGraphics.CGPoint(0, 0);
        node.Position = new CoreGraphics.CGPoint(scenePos.X, scenePos.Y);
        node.ZPosition = zPos ?? dest.ZPosition;
        dest.AddChild(node);
      }
    }
  }
}
