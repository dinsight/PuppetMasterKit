using System;
using System.Linq;
using CoreGraphics;
using CoreImage;
using Foundation;
using LightInject;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Ios.Isometric.Tilemap
{
  public class TileMapLayer : SKNode
  {
    private const int maxSliceSize = 4000;
    private WeakReference<TileMap> map;
    private ICoordinateMapper mapper;
    private ImageHelper tileHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Tilemap.TileMapLayer"/> class.
    /// </summary>
    /// <param name="map">Map.</param>
    internal TileMapLayer(TileMap map)
    {
      this.map = new WeakReference<TileMap>(map);
      this.mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
      this.tileHelper = new ImageHelper();
    }

    /// <summary>
    /// Sets the tile.
    /// </summary>
    /// <param name="texture">Texture.</param>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    public void SetTile(SKTexture texture, int row, int col, float? zPos =null )
    {
      if (!Validate(row, col) || texture==null)
        return;
      var x = (row + 1) * GetMap().TileSize;
      var y = (col + 1) * GetMap().TileSize;
      var pos = new Point(x, y);
      var scenePos = mapper.ToScene(pos);
      if(texture!=null){
        var node = SKSpriteNode.FromTexture(texture);
        node.AnchorPoint = new CoreGraphics.CGPoint(0.5, 0);
        node.Position = new CoreGraphics.CGPoint(scenePos.X, scenePos.Y);
        node.ZPosition = zPos ?? ZPosition;
        this.AddChild(node);
      }
    }

    /// <summary>
    /// Sets the texture.
    /// </summary>
    /// <param name="texture">Texture.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public void SetTexture(SKTexture texture, float x, float y, float? zPos = null){
      
      if (texture != null) {
        var node = SKSpriteNode.FromTexture(texture);
        //BUG : anchor point is not working properly
        //We substract half the width so the image gets centered on the x axis
        //var scenePos = mapper.ToScene(new Point(x+(float)node.Size.Width/2, y + GetMap().TileSize/2));
        var scenePos = mapper.ToScene(new Point(x, y));
        node.AnchorPoint = new CoreGraphics.CGPoint(0, 0);
        node.Position = new CoreGraphics.CGPoint(scenePos.X, scenePos.Y);
        node.ZPosition = zPos ?? ZPosition;
        this.AddChild(node);
      }
    }

    /// <summary>
    /// Converts to texture.
    /// </summary>
    /// <returns>The to texture.</returns>
    public SKSpriteNode FlattenLayer()
    {
      var image = tileHelper.FlattenNode(this, GetMap().TileSize, GetMap().Rows, GetMap().Cols);
      var newNode = tileHelper.SplitImage(image, maxSliceSize, maxSliceSize);
#if DEBUG
      tileHelper.SaveImage(image, "/Users/alexjecu/Desktop/Workspace/dinsight/xamarin/assets/map.png");
#endif
      RemoveAllChildren();
      AddChild(newNode);
      newNode.Position = new CGPoint(0, 0);
      newNode.AnchorPoint = new CGPoint(0.5, 1);
      newNode.Size = new CGSize(image.Width, image.Height);
      return newNode;
    }

    /// <summary>
    /// Gets the map.
    /// </summary>
    /// <returns>The map.</returns>
    private TileMap GetMap(){
      TileMap tileMap = null;
      if (!map.TryGetTarget(out tileMap)){
        throw new ArgumentException("TileMapLayer: Invalid map reference");
      }
      return tileMap;
    }

    /// <summary>
    /// Validate the specified row and col.
    /// </summary>
    /// <returns>The validate.</returns>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    private bool Validate(int row, int col)
    {
      var tileMap = GetMap();
      if(row < 0 || row >= tileMap.Rows || col < 0 || col >= tileMap.Cols){
        return false;
      }
      return true;
    }
  }
}
