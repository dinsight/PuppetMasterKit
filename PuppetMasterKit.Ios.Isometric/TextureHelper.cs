using System;
using System.Linq;
using CoreImage;
using SpriteKit;

namespace PuppetMasterKit.Ios.Isometric
{
  public static class TextureHelper
  {

    static Random randomTexture = new Random(Guid.NewGuid().GetHashCode());

    /// <summary>
    /// Gets the texture.
    /// </summary>
    /// <returns>The texture.</returns>
    /// <param name="group">Group.</param>
    /// <param name="ruleName">Rule name.</param>
    public static SKTexture GetTexture(this SKTileGroup group, string ruleName){
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
    /// Blends the with alpha.
    /// </summary>
    /// <returns>The with alpha.</returns>
    /// <param name="texture">Texture.</param>
    /// <param name="textureAlpha">Texture alpha.</param>
    public static SKTexture BlendWithAlpha(this SKTexture texture, SKTexture textureAlpha)
    {
      var blend = new CIBlendWithAlphaMask() {
        BackgroundImage = null,
        Image = texture.CGImage,
        Mask = textureAlpha.CGImage
      };
      var output = blend.OutputImage;
      var context = CIContext.FromOptions(null);
      var cgimage = context.CreateCGImage(output, output.Extent);
      var blendedTexture = SKTexture.FromImage(cgimage);
      return blendedTexture;
    }
  }
}
