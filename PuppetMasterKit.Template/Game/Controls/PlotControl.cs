using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CoreGraphics;
using CoreHaptics;
using Foundation;
using LightInject;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Ios.Tiles.Tilemap;
using PuppetMasterKit.Template.Game.Controls.Buttons;
using PuppetMasterKit.Template.Game.Controls.Gestures;
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
    public Action<PlotControl,String> OnOk { get; set; }
    private readonly NSString IsMultiselect = new NSString("isMultiselect");

    private CGPoint initialPosition;
    private SKScene scene;
    private TileMap tileMap;
    private ICoordinateMapper mapper;
    private CustomButton menuNode = null;
    private SKSpriteNode floatMenu = null;
    private bool isPositioned = false;
    
    #region Gesture Recognizers
    private UIGestureRecognizer[] savedRecognizers;
    private UILongPressGestureRecognizer longPress;
    private TapWithTouchGestureRecognizer tap;
    private UIPanGestureRecognizer pan;
    private UISwipeGestureRecognizer swipeUpGesture;
    private UISwipeGestureRecognizer swipeUpOverToolsGesture;
    #endregion

    private Dictionary<Pair, SKShapeNode> selected = new Dictionary<Pair, SKShapeNode>();

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    private PlotControl(IntPtr handle) : base(handle) {
      UserInteractionEnabled = true;
      mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
      Initialize();
    }

    /// <summary>
    /// 
    /// </summary>
    private void Initialize() {
      menuNode = Children.FirstOrDefault(x => x.Name == "menu") as CustomButton;
      floatMenu = menuNode.Children.FirstOrDefault(x => x.Name == "float") as SKSpriteNode;
      floatMenu.Paused = false;
      floatMenu.AnchorPoint = new CGPoint(0,1);
      this.Paused = false;

      swipeUpGesture = new UISwipeGestureRecognizer(OnSwipeUp) {
        Direction = UISwipeGestureRecognizerDirection.Up
      };
      swipeUpOverToolsGesture = new SwipeOverSpriteGestureRecognizer(floatMenu, OnSwipeUpOverMenu) {
         Direction = UISwipeGestureRecognizerDirection.Up
      };
      pan = new UIPanGestureRecognizer(OnPan);
      longPress = new LongPressWithTouchGestureRecognizer(OnLongPress);
      tap = new TapWithTouchGestureRecognizer(OnTap) {
          
      };
      

      floatMenu.UserInteractionEnabled = false;
      GetAllButtonsForNode(menuNode).ForEach(button => {
        button.OnButtonPressed += Item_OnButtonPressed;
        button.OnButtonReleased += Item_OnButtonReleased;
      });

      this.Shader = SKShader.FromFile("Shaders/Glass.fsh");
  }

    /// <summary>
    /// 
    /// </summary>
    public void Open()
    {
      scene.Camera.AddChild(this);
      UpdateControlPosition();
      SetGestureRecognizers();
      GetAllButtonsForNode(menuNode)
        .OfType<ToggleButton>().ForEach(item => 
            item.ToggleState(false));

      var hud = Container.GetContainer().GetInstance<Hud>();
      hud.SetMessage("Swipe up to close the menu and confirm your changes. " +
        "Double tap on the buttons to reset your changes. ");
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetGestureRecognizers()
    {
      scene.View.AddGestureRecognizer(tap);
      scene.View.AddGestureRecognizer(longPress);
      scene.View.AddGestureRecognizer(pan);
      scene.View.AddGestureRecognizer(swipeUpGesture);
      scene.View.AddGestureRecognizer(swipeUpOverToolsGesture);

      swipeUpGesture.ShouldRequireFailureOf = (gesture, otherGesture) =>
        gesture == swipeUpGesture && (
              otherGesture == swipeUpOverToolsGesture
              );

      tap.ShouldRequireFailureOf = (gesture, otherGesture) =>
        gesture == tap && (
              otherGesture == swipeUpOverToolsGesture ||
              otherGesture == swipeUpGesture
              );

      pan.ShouldRequireFailureOf = (gesture, otherGesture) =>
        gesture == pan && (
              otherGesture == swipeUpGesture ||
              otherGesture == swipeUpOverToolsGesture);
    }

    /// <summary>
    /// 
    /// </summary>
    private void RestoreGestureRecognizers()
    {
      scene.View.RemoveGestureRecognizer(tap);
      scene.View.RemoveGestureRecognizer(longPress);
      scene.View.RemoveGestureRecognizer(pan);
      scene.View.RemoveGestureRecognizer(swipeUpGesture);
      scene.View.RemoveGestureRecognizer(swipeUpOverToolsGesture);
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
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uiGesture"></param>
    private void OnTap(UITapGestureRecognizer uiGesture)
    {
      var gesture = uiGesture as TapWithTouchGestureRecognizer;
      if (uiGesture.State == UIGestureRecognizerState.Ended) {
        if (HasButtonPressed()) {
          DoLongPressHapticEffect();
          SelectTouchedTiles(gesture.Touches);
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gesture"></param>
    private void OnSwipeUp(UISwipeGestureRecognizer gesture)
    {
      Close();
      OnOk?.Invoke(this, GetSelectedButton()?.Name);
      var hud = Container.GetContainer().GetInstance<Hud>();
      hud.SetMessage(String.Empty);
    }

    private void OnSwipeUpOverMenu(SwipeOverSpriteGestureRecognizer gesture)
    {
      floatMenu = menuNode.Children.FirstOrDefault(x => x.Name == "float") as SKSpriteNode;
      var p = this.ConvertPointFromNode(floatMenu.Position, floatMenu);

      const int step = 50;
      var actualStep = floatMenu.Position.Y + step <= this.Size.Height ? step : this.Size.Height - floatMenu.Position.Y;

      if (actualStep > 0) {
        var action = SKAction.MoveBy(0, actualStep, 0.5);
        var group = SKAction.Sequence(SKAction.Run(() =>
          {
              var check = floatMenu.Paused;
          }), action);
        System.Diagnostics.Debug.WriteLine($"OnSwipeUpOverMenu {actualStep}. Running action. Is paused {floatMenu.Paused}");
        floatMenu.RunAction(group);
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public void Close()
    {
      RestoreGestureRecognizers();
      scene.Camera.Position = initialPosition;
      this.RemoveFromParent();
    }

    /// <summary>
    /// 
    /// </summary>
    public void ClearSelection() {
      selected.ForEach(sel => {
        sel.Value.RemoveFromParent();
      });
      selected.Clear();
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
    /// </summary>
    private void UpdateControlPosition()
    {
      this.Position = new CGPoint(0, 0);
      this.AnchorPoint = new CGPoint(0.5, 0.5);
      this.ZPosition = 1000;
      this.Size = ControlsUtil.GetVisibleScreenSize(scene);

      initialPosition = new CGPoint(scene.Camera.Position.X, scene.Camera.Position.Y);
      floatMenu.Position = new CGPoint(
        -this.Size.Width / 2 ,
        this.Size.Height / 2 );
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool HasButtonPressed() {
      return GetAllButtonsForNode(menuNode)
        .OfType<ToggleButton>().Any(btn => btn.IsPressed);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private ToggleButton GetSelectedButton()
    {
      return GetAllButtonsForNode(menuNode)
        .OfType<ToggleButton>().FirstOrDefault(btn => btn.IsPressed);
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
      var button = sender as ToggleButton;
      if (button != null && button.IsPressed) {
        GetAllButtonsForNode(menuNode)
        .OfType<ToggleButton>().ForEach(item => {
          if (item.Name != button.Name) {
            item.ToggleState(false);
          }
        });
        ClearSelection();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Item_OnButtonReleased(object sender, EventArgs e)
    {
      var button = sender as ToggleButton;
      if (button != null && !button.IsPressed) {
        ClearSelection();
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
          ClearSelection();
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
