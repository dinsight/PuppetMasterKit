using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using CoreHaptics;
using Foundation;
using LightInject;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Ios.Tiles.Tilemap;
using PuppetMasterKit.Template.Game.Gestures;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.Utility.Extensions;
using SpriteKit;
using UIKit;
using Pair = System.Tuple<int, int>;

namespace PuppetMasterKit.Template.Game.Controls
{
  public class PlotControl : SKSpriteNode
  {
    public event Func<String, bool> OnItemButtonClick;
    public Action<PlotControl> OnClosing { get; set; }
    public Action<PlotControl> OnOk { get; set; }

    private readonly int Padding = 10;
    private CGPoint initialPosition;
    private SKScene scene;
    private TileMap tileMap;
    private ICoordinateMapper mapper;
    private CustomButton menuNode = null;
    private SKSpriteNode floatMenu = null;
    private bool isPositioned = false;
    private UILongPressGestureRecognizer longPress;
    private UIPanGestureRecognizer pan;
      
    private Dictionary<Pair, SKShapeNode> selected = new Dictionary<Pair, SKShapeNode>();
    private readonly NSString IsMultiselect = new NSString("isMultiselect");

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="tileMap"></param>
    /// <param name="fromFile"></param>
    /// <param name="controlName"></param>
    /// <returns></returns>
    public static PlotControl CreateFromFile(SKScene scene, TileMap tileMap,
      string fromFile,
      string controlName)
    {
      var scn = SKNode.FromFile<SKScene>(fromFile);
      var ctrl = scn.Children.FirstOrDefault(x => x.Name == controlName) as PlotControl;
      ctrl.scene = scene;
      ctrl.tileMap = tileMap;
      ctrl.RemoveFromParent();
      return ctrl;
    }

#pragma warning disable IDE0051 // Remove unused private members
    private PlotControl(IntPtr handle) : base(handle) {
#pragma warning restore IDE0051 // Remove unused private members
      UserInteractionEnabled = true;
      mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
      pan = new UIPanGestureRecognizer(OnPan);
      longPress = new LongPressWithTouchGestureRecognizer(OnLongPress);

      menuNode = Children.FirstOrDefault(x => x.Name == "menu") as CustomButton;
      floatMenu = menuNode.Children.FirstOrDefault(x => x.Name == "float") as SKSpriteNode;
      
      floatMenu.BecomeFirstResponder();
      GetAllButtonsForNode(menuNode).ForEach(button => {
        button.OnButtonPressed += Item_OnButtonPressed;
      });
    }

    /// <summary>
    /// 
    /// </summary>
    public void Open()
    {
      scene.Camera.AddChild(this);
      UpdateControlPosition();
      AddGestureRecognizers();
      //Container.GetContainer().GetInstance<Hud>().Hidden = true;
    }

    /// <summary>
    /// 
    /// </summary>
    private void AddGestureRecognizers()
    {
      scene.View.AddGestureRecognizer(longPress);
      scene.View.AddGestureRecognizer(pan);
    }

    /// <summary>
    /// 
    /// </summary>
    private void RemoveGestureRecognizers()
    {
      scene.View.RemoveGestureRecognizer(longPress);
      scene.View.RemoveGestureRecognizer(pan);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gesture"></param>
    private void OnPan(UIPanGestureRecognizer gesture) {
      var translation = gesture.TranslationInView(scene.View);
      scene.Camera.Position = new CGPoint(
          scene.Camera.Position.X - translation.X,
          scene.Camera.Position.Y + translation.Y);
      gesture.SetTranslation(new CGPoint(0, 0), scene.View);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gesture"></param>
    private void OnLongPress(UILongPressGestureRecognizer uiGesture)
    {
      var gesture = uiGesture as LongPressWithTouchGestureRecognizer;
      var hud = Container.GetContainer().GetInstance<Hud>();
      DoLongPressHapticEffect();
      SelectTouchedTiles(gesture.Touches);
      RemoveGestureRecognizers();
      OnOk?.Invoke(this);
      //hud.Hidden = false;
      scene.Camera.Position = initialPosition;
      this.RemoveFromParent();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Close() {
      var hud = Container.GetContainer().GetInstance<Hud>();
      RemoveGestureRecognizers();
      OnClosing?.Invoke(this);
      selected.Clear();
      //hud.Hidden = false;
      scene.Camera.Position = initialPosition;
      this.RemoveFromParent();
    }

    /// <summary>
    /// 
    /// </summary>
    private static void DoLongPressHapticEffect()
    {
      var caps = CHHapticEngine.GetHardwareCapabilities();
      if (caps.SupportsHaptics) {
        NSError error;
        var eng = new CHHapticEngine(out error);
        var haptic = new CHHapticEvent(CHHapticEventType.HapticContinuous,
          new CHHapticEventParameter[] {
            new CHHapticEventParameter(CHHapticEventParameterId.HapticIntensity, value: 0.55f),
            new CHHapticEventParameter(CHHapticEventParameterId.HapticSharpness, value: 0.55f)
        }, 0, 0.45);
        var pattern = new CHHapticPattern(new CHHapticEvent[] { haptic },
                      new CHHapticDynamicParameter[] { }, out error);

        var player = eng.CreatePlayer(pattern, out error);
        eng.Start(x => {
          player.Start(0, out error);
        });
      }
    }

    /// <summary>
    /// 
    /// </summary>xx
    /// <returns></returns>
    private double GetAspectFillScaleFactor() {
      var screenSize = UIScreen.MainScreen.Bounds.Size;
      var sceneSize = scene.Frame.Size;
      var factor = 1.0;
      if (screenSize.Height > sceneSize.Height) {
        factor = screenSize.Height / sceneSize.Height;
      } else if (screenSize.Width > sceneSize.Width) {
        factor = screenSize.Width / sceneSize.Width;
      }
      return factor;
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateControlPosition()
    {
      if (isPositioned)
        return;
      isPositioned = true;

      var size = UIScreen.MainScreen.Bounds.Size;
      var factor = GetAspectFillScaleFactor();
      this.Position = new CGPoint(0, 0);
      this.AnchorPoint = new CGPoint(0.5, 0.5);
      this.ZPosition = 1000;
      this.Size = new CGSize(size.Width / factor - Padding, size.Height / factor - Padding);

      initialPosition = new CGPoint(scene.Camera.Position.X, scene.Camera.Position.Y);
      floatMenu.Position = new CGPoint(
        this.Size.Width / 2 - floatMenu.Size.Width / 2,
        this.Size.Height / 2 - floatMenu.Size.Height / 2);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buttonName"></param>
    private void TurnOffOtherToggleButtons(string buttonName) {
      GetAllButtonsForNode(menuNode).OfType<ToggleButton>().ForEach(item => {
        if (item.Name != buttonName) {
          item.ToggleState(false);
        }
      });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool HasButtonPressed() {
      return GetAllButtonsForNode(menuNode).OfType<ToggleButton>().Any(btn => btn.IsPressed);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private ToggleButton GetSelectedButton()
    {
      return menuNode.Children.OfType<ToggleButton>().FirstOrDefault(btn => btn.IsPressed);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private IEnumerable<CustomButton> GetAllButtonsForNode(SKNode node)
    {
      foreach (var item in node.Children.OfType<SKNode>()) {
        var button = item as CustomButton;
        if (button != null)
          yield return button;
        foreach (var item2 in GetAllButtonsForNode(item)) {
          yield return item2;
        }
      }
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
          //hud.Hidden = false;
          scene.Camera.Position = initialPosition;
          this.RemoveFromParent();
          
        }
        if (isOk || isItemClick) OnOk?.Invoke(this);
        if (isCancel) OnClosing?.Invoke(this);

        selected.Clear();
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
    private void SelectTouchedTiles(NSSet touches)
    {
      var positions = new List<CGPoint>();
      foreach (UITouch touch in touches) {
        var layer = tileMap.GetLayer(0);
        positions.Add(touch.LocationInNode(layer));
      }
      SelectTileAtPositions(positions.ToArray());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="touches"></param>
    private void SelectTileAtPositions(CGPoint[] positions)
    {
      foreach (CGPoint positionInScene in positions) {
        var coord2d = mapper.FromScene(new Point((float)positionInScene.X, (float)positionInScene.Y));
        if (coord2d.X < 0 || coord2d.Y < 0) {
          break;
        }

        var row = (int)(coord2d.Y / tileMap.TileSize);
        var col = (int)(coord2d.X / tileMap.TileSize);
        if (row < 0 || col < 0 || row >= tileMap.Rows || col >= tileMap.Cols) {
          break;
        }

        var isMulti = GetSelectedButton()?.UserData?.ContainsKey(IsMultiselect) ?? false;
        if (!isMulti) {
          selected.Values.ForEach(x => x.RemoveFromParent());
          selected.Clear();
        }

        var key = selected.Keys.Where(x => x.Item1 == row && x.Item2 == col).FirstOrDefault();
        if (key != null) {
          var val = selected.Where(x => x.Key == key).First();
          selected.Remove(key);
          val.Value.RemoveFromParent();
        } else {
          var square = HighlightTile(row, col, true);
          tileMap.GetLayer(tileMap.LayerCount - 1).AddChild(square);
          selected.Add(new Pair(row, col), square);
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <param name="highlight"></param>
    private SKShapeNode HighlightTile(int row, int col, bool highlight)
    {
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
