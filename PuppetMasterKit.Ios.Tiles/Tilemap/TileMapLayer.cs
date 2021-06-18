using System;
using System.Linq;
using CoreGraphics;
using CoreImage;
using Foundation;
using LightInject;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Ios.Tiles.Tilemap.Helpers;
using SpriteKit;
using UIKit;
using PuppetMasterKit.Utility;
using System.Collections.Generic;

namespace PuppetMasterKit.Ios.Tiles.Tilemap
{
  public class TileMapLayer : SKNode
  {
    private const int maxSliceSize = 4000;
    private int index;
    private WeakReference<TileMap> map;
    private Dictionary<GridCoord, SKSpriteNode> dictionary;
    static ICoordinateMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Tilemap.TileMapLayer"/> class.
    /// </summary>
    /// <param name="map">Map.</param>
    internal TileMapLayer(TileMap map, int index)
    {
      this.index = index;
      this.map = new WeakReference<TileMap>(map);
      mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
      dictionary = new Dictionary<GridCoord, SKSpriteNode>();
    }

    /// <summary>
    /// Sets the tile.
    /// </summary>
    /// <param name="texture">Texture.</param>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    public SKNode SetTile(SKTexture texture, int row, int col, float? zPos, CGPoint? anchorPoint=null)
    {
      if (!Validate(row, col) || texture==null)
        return null;
      var tileSize = GetMap().TileSize;
      var x = (row + 0) * tileSize + tileSize / 2;
      var y = (col + 0) * tileSize + tileSize / 2;
      var pos = new Point(x, y);
      var scenePos = mapper.ToScene(pos);
      if(texture!=null){
        var coord = new GridCoord(row, col);
        if (dictionary.ContainsKey(coord)) {
          dictionary[coord].RemoveFromParent();
          dictionary.Remove(coord);
        }

        var node = SKSpriteNode.FromTexture(texture);
        node.AnchorPoint = anchorPoint ?? new CGPoint(0.5,0.5);
        node.Position = new CoreGraphics.CGPoint(scenePos.X, scenePos.Y);
        node.ZPosition = zPos ?? ZPosition;
        this.AddChild(node);
        dictionary.TryAdd(new GridCoord(row,col), node);
        return node;
      }
      return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public SKNode GetTile(int row, int col) {
      var coord = new GridCoord(row, col);
      if (!Validate(row, col) || !dictionary.ContainsKey(coord))
        return null;
      return dictionary[coord];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    public void ClearTile(int row, int col)
    {
      var coord = new GridCoord(row, col);
      if (!Validate(row, col) || !dictionary.ContainsKey(coord))
        return;
      if (dictionary.ContainsKey(coord)) {
        dictionary[coord].RemoveFromParent();
        dictionary.Remove(coord);
      }
    }

    /// <summary>
    /// Converts to texture.
    /// </summary>
    /// <returns>The to texture.</returns>
    public SKSpriteNode FlattenLayer(Action<CGImage> debug = null)
    {
      var image = ImageHelper.FlattenNode(this, GetMap().TileSize, GetMap().Rows, GetMap().Cols);
      var newNode = image.SplitImage(maxSliceSize, maxSliceSize);
      debug?.Invoke(image);
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
    public TileMap GetMap(){
      if (!map.TryGetTarget(out TileMap tileMap)) {
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
