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
using PuppetMasterKit.Utility.Extensions;
using Pair = System.Tuple<int, int>;
using System.Collections.Generic;
using System.Reflection;

namespace PuppetMasterKit.Template.Game.Controls
{
  public class PlotControl : SKSpriteNode
  {
    private readonly NSString IS_MULTISELECT = new NSString("isMultiselect");

    public event Func<String, bool> OnItemButtonClick;
    public Action<PlotControl> OnClosing { get; set; }
    public Action<PlotControl> OnOk { get; set; }

    private bool isPanning = false;
    private CGPoint initialPosition;
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
      return ctrl;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Edit() {
      scene.Camera.AddChild(this);
      var size = scene.Frame;
      this.Position = new CGPoint(0, 0);
      this.AnchorPoint = new CGPoint(0.5, 0.5);
      this.ZPosition = 1000;
      this.Size = new CGSize(size.Width, size.Height);
      initialPosition = new CGPoint(scene.Camera.Position.X, scene.Camera.Position.Y);

      RepositionControls();

      var hud = Container.GetContainer().GetInstance<Hud>();
      hud.Hidden = true;
    }

    /// <summary>
    /// 
    /// </summary>
    private void RepositionControls() {
      var size = scene.Frame;
      var menuNode = this.Children.FirstOrDefault(x => x.Name == "menu") as CustomButton;
      var calcFrame = this.CalculateAccumulatedFrame();
      var scalex = size.Width/ calcFrame.Width;
      var scaley = size.Height/ calcFrame.Height;

      foreach (var item in menuNode.Children.OfType<CustomButton>()) {
        item.OnButtonPressed += Item_OnButtonPressed;
        item.Position = new CGPoint(item.Position.X * scalex, item.Position.Y * scaley);
        item.UpdateLayout();
      }

      var okButton = menuNode.Children.OfType<PlainButton>().Where(x => x.Name == "Ok").First();
      var cancelButton = menuNode.Children.OfType<PlainButton>().Where(x => x.Name == "Cancel").First();

      //okButton.Position = new CGPoint( okButton.Position.X, 100);
      //cancelButton.Position = new CGPoint(cancelButton.Position.X, 100);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buttonName"></param>
    private void TurnOffOtherToggleButtons(string buttonName) {
      var menuNode = this.Children.FirstOrDefault(x => x.Name == "menu") as CustomButton;
      foreach (var item in menuNode.Children.OfType<ToggleButton>()) {
        if (item.Name != buttonName) {
          item.ToggleState(false);
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool HasButtonPressed() {
      var menuNode = this.Children.FirstOrDefault(x => x.Name == "menu") as CustomButton;
      return menuNode.Children.OfType<ToggleButton>().Any(btn => btn.IsPressed);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private ToggleButton GetSelectedButton()
    {
      var menuNode = this.Children.FirstOrDefault(x => x.Name == "menu") as CustomButton;
      return menuNode.Children.OfType<ToggleButton>().FirstOrDefault(btn => btn.IsPressed);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Item_OnButtonPressed(object sender, EventArgs e)
    {
      var button = sender as CustomButton;
      TurnOffOtherToggleButtons(button.Name);
      var hud = Container.GetContainer().GetInstance<Hud>();
      if (OnItemButtonClick != null) {
        var isOk = button.Name == "Ok";
        var isCancel = button.Name == "Cancel";
        var isItemClick = false;
        if (isOk || isCancel || (isItemClick = OnItemButtonClick(button.Name))) {
          hud.Hidden = false;
          scene.Camera.Position = initialPosition;
          this.RemoveFromParent();
        }

        if (isOk || isItemClick) OnOk?.Invoke(this);
        if (isCancel) OnClosing?.Invoke(this);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public List<Pair> GetSelectedTiles() {
      return selected.Keys.ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="touches"></param>
    /// <param name="evt"></param>
    public override void TouchesBegan(NSSet touches, UIEvent evt)
    {
      isPanning = false;
      base.TouchesBegan(touches, evt);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="touches"></param>
    /// <param name="evt"></param>
    public override void TouchesMoved(NSSet touches, UIEvent evt)
    {
      isPanning = true;
      var touch = touches.AnyObject as UITouch;
      var positionInScene = touch.LocationInNode(this);
      var previousPosition = touch.PreviousLocationInNode(this);
      var translation = new CGPoint(x: positionInScene.X - previousPosition.X,
        y: positionInScene.Y - previousPosition.Y);
      scene.Camera.Position = new CGPoint(scene.Camera.Position.X - translation.X,
        scene.Camera.Position.Y - translation.Y);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="touches"></param>
    /// <param name="evt"></param>
    public override void TouchesEnded(NSSet touches, UIEvent evt)
    {
      if (isPanning || !HasButtonPressed())
        return;

      var mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
      foreach (UITouch touch in touches) {
        var layer = tileMap.GetLayer(0);
        var positionInScene = touch.LocationInNode(layer);
        var coord2d = mapper.FromScene(new Point((float)positionInScene.X, (float)positionInScene.Y));

        if (coord2d.X < 0 || coord2d.Y < 0) {
          break;
        }

        var row = (int) (coord2d.Y / tileMap.TileSize);
        var col = (int) (coord2d.X / tileMap.TileSize);

        if (row < 0 || col < 0 || row >= tileMap.Rows || col >= tileMap.Cols) {
          break;
        }

        var isMulti = GetSelectedButton()?.UserData?.ContainsKey(IS_MULTISELECT) ?? false;
        if (!isMulti) {
          selected.Values.ForEach(x=>x.RemoveFromParent());
          selected.Clear();
        }

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
