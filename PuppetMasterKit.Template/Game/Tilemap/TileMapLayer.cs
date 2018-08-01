using System;
using System.Linq;
using LightInject;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;

namespace PuppetMasterKit.Tilemap
{
  public class TileMapLayer : SKNode
  {
    private WeakReference<TileMap> map;

    private ICoordinateMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Tilemap.TileMapLayer"/> class.
    /// </summary>
    /// <param name="map">Map.</param>
    internal TileMapLayer(TileMap map)
    {
      this.map = new WeakReference<TileMap>(map);
      this.mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
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
      var x = (row + 1) * GetMap().TileWidth;
      var y = (col + 1) * GetMap().TileHeight;
      var pos = new Point(x, y);
      var scenePos = mapper.ToScene(pos);
      if(texture!=null){
        var node = SKSpriteNode.FromTexture(texture);
        node.AnchorPoint = new CoreGraphics.CGPoint(0.5, 0);
        node.Position = new CoreGraphics.CGPoint(scenePos.X, scenePos.Y);
        node.ZPosition = zPos.HasValue ? zPos.Value : ZPosition;
        this.AddChild(node);
      }
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
