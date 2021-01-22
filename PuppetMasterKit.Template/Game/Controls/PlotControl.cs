using System;
using LightInject;
using System.Linq;
using CoreGraphics;
using Foundation;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Ios.Tiles.Tilemap;
using SpriteKit;
using UIKit;
using PuppetMasterKit.Utility.Configuration;
using System.Diagnostics;
using Pair = System.Tuple<int, int>;
using System.Collections.Generic;

namespace PuppetMasterKit.Template.Game.Controls
{
  public class PlotControl : SKSpriteNode
  {
    private SKScene scene;
    private TileMap tileMap;
    private Dictionary<Pair, SKShapeNode> selected = new Dictionary<Pair, SKShapeNode>();

#pragma warning disable IDE0051 // Remove unused private members
    private PlotControl(IntPtr handle) : base(handle) {
#pragma warning restore IDE0051 // Remove unused private members
    }

    /// <summary>
    /// 
    /// </summary>
    public void Edit() {
      scene.Camera.AddChild(this);
      this.Position = new CGPoint(0.5, 0.5);
      this.AnchorPoint = new CGPoint(0.5, 0.5);
      this.ZPosition = 1000;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="tileMap"></param>
    /// <param name="fromFile"></param>
    /// <param name="controlName"></param>
    /// <returns></returns>
    public static PlotControl Create(SKScene scene, TileMap tileMap,
      string fromFile,
      string controlName)
    {
      var scn = SKNode.FromFile<SKScene>(fromFile);
      var ctrl = scn.Children.FirstOrDefault(x => x.Name == controlName) as PlotControl;
      ctrl.scene = scene;
      ctrl.tileMap = tileMap;
      ctrl.RemoveFromParent();
      ctrl.UserInteractionEnabled = true;
      var size = UIScreen.MainScreen.Bounds;
      ctrl.Size = new CGSize(size.Width, size.Height);
      return ctrl;
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="touches"></param>
    /// <param name="evt"></param>
    public override void TouchesEnded(NSSet touches, UIEvent evt)
    {
      var mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
      foreach (UITouch touch in touches) {
        var layer = tileMap.GetLayer(0);
        var positionInScene = touch.LocationInNode(layer);
        var coord2d = mapper.FromScene(new Point((float)positionInScene.X, (float)positionInScene.Y));

        var row = (int) (coord2d.Y / tileMap.TileSize);
        var col = (int) (coord2d.X / tileMap.TileSize);

        var key = selected.Keys.Where(x => x.Item1 == row && x.Item2 == col).FirstOrDefault();
        if (key!=null) {
          var val = selected.Where(x => x.Key == key).First();
          selected.Remove(key);
          val.Value.RemoveFromParent();
        } else {
          var square = Highlight(row, col, true);
          tileMap.GetLayer(tileMap.LayerCount - 1).AddChild(square);
          selected.Add(new Pair(row, col), square);
        }
        
      }
      base.TouchesEnded(touches, evt);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <param name="highlight"></param>
    private SKShapeNode Highlight(int row, int col, bool highlight) {

      var mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
      var w = tileMap.TileSize;
      var h = tileMap.TileSize / 2;
      var square = SKShapeNode.FromPoints(new CGPoint[]{
        new CGPoint(-w/2,h/2),
        new CGPoint(0,0),
        new CGPoint(w/2,h/2),
        new CGPoint(0,h),
      });
      var dict = new NSMutableDictionary();

      square.FillColor = UIColor.Red;
      square.Alpha = 0.3f;
      square.ZPosition = 10;

      var x = (col + 1) * tileMap.TileSize;
      var y = (row + 1) * tileMap.TileSize;
      var scenePos = mapper.ToScene(new Point(x, y));
      square.Position = new CGPoint(scenePos.X, scenePos.Y);
      
      return square;
    }
  }
}
