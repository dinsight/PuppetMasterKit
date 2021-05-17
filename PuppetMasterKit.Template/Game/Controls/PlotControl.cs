using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CoreGraphics;
using CoreHaptics;
using Foundation;
using LightInject;
using OpenTK;
using PuppetMasterKit.AI;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Ios.Tiles.Tilemap;
using PuppetMasterKit.Template.Game.Controls.Buttons;
using PuppetMasterKit.Template.Game.Controls.Gestures;
using PuppetMasterKit.Utility;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.Utility.Extensions;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template.Game.Controls
{
  public class PlotControl : SKSpriteNode
  {
    public event Func<PlotControl, GridCoord, bool> SelectionValidator;
    public event Func<String, bool> OnItemButtonClick;
    public Action<PlotControl,String> OnOk { get; set; }

    private readonly NSString IsMultiselect = new NSString("isMultiselect");
    private int marginTop;
    private CGPoint initialPosition;
    private SKScene scene;
    private TileMap tileMap;
    private ICoordinateMapper mapper;
    private CustomButton menuNode = null;
    private SKSpriteNode floatMenu = null;
    private ToggleButton helpNode = null;
    private Dictionary<GridCoord, SKShapeNode> selected = new Dictionary<GridCoord, SKShapeNode>();

    #region Gesture Recognizers
    private UILongPressGestureRecognizer longPress;
    private TapWithTouchGestureRecognizer tap;
    private UIPanGestureRecognizer pan;
    private UISwipeGestureRecognizer swipeUpGesture;
    private UISwipeGestureRecognizer swipeUpOverToolsGesture;
    private UISwipeGestureRecognizer swipeDownOverToolsGesture;
    #endregion

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
      Container.GetContainer().RegisterInstance<PlotControl>(ctrl); 
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
      helpNode = Children.FirstOrDefault(x => x.Name == "help") as ToggleButton;
      
      this.Paused = false;

      swipeUpGesture = new UISwipeGestureRecognizer(OnSwipeUp) {
        Direction = UISwipeGestureRecognizerDirection.Up
      };
      
      pan = new UIPanGestureRecognizer(OnPan);
      longPress = new LongPressWithTouchGestureRecognizer(OnLongPress);
      tap = new TapWithTouchGestureRecognizer(OnTap);

      GetAllButtonsForNode(menuNode).ForEach(button => {
        button.OnButtonPressed += Item_OnButtonPressed;
        button.OnButtonReleased += Item_OnButtonReleased;
      });
      helpNode.OnButtonPressed += HelpNode_OnButtonPressed;
      helpNode.OnButtonReleased += HelpNode_OnButtonReleased;
      Shader = null;

      var isLandscape = UIApplication.SharedApplication.StatusBarOrientation.IsLandscape();
      marginTop = isLandscape ? 10 : 50;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HelpNode_OnButtonReleased(object sender, EventArgs e)
    {
      var hud = Container.GetContainer().GetInstance<Hud>();
      hud.SetMessage(String.Empty);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HelpNode_OnButtonPressed(object sender, EventArgs e)
    {
      var hud = Container.GetContainer().GetInstance<Hud>();
      hud.SetMessage("- <Swipe Up> to close the menu\n" +
        "- <Touch and Hold> to start building\n" +
        "- <Pan> to move the map ");
      
    }

    /// <summary>
    /// 
    /// </summary>
    public void Open(Entity forEntity)
    {
      floatMenu = menuNode.Children.FirstOrDefault(x => x.Name == forEntity.Name) as SKSpriteNode;
      if (floatMenu == null)
        throw new Exception($"No context menu for {forEntity.Name}");

      menuNode.Children.Where(x => x.Name != forEntity.Name).ForEach(c=>c.Hidden=true);

      scene.Camera.AddChild(this);
      floatMenu.Paused = false;
      floatMenu.UserInteractionEnabled = false;

      if (swipeUpOverToolsGesture != null) {
        scene.View.AddGestureRecognizer(swipeUpOverToolsGesture);
      }
      if (swipeDownOverToolsGesture != null) {
        scene.View.AddGestureRecognizer(swipeDownOverToolsGesture);
      }
      
      swipeUpOverToolsGesture = new SwipeOverSpriteGestureRecognizer(floatMenu, OnSwipeOverMenu) {
        Direction = UISwipeGestureRecognizerDirection.Up,
        MinGestureSize = 25
      };
      swipeDownOverToolsGesture = new SwipeOverSpriteGestureRecognizer(floatMenu, OnSwipeOverMenu) {
        Direction = UISwipeGestureRecognizerDirection.Down,
        MinGestureSize = 25
      };

      UpdateControlPosition();
      SetGestureRecognizers();
      GetAllButtonsForNode(menuNode)
        .OfType<ToggleButton>().ForEach(item => 
            item.ToggleState(false));

      if (Shader == null) {
        var screenSize = ControlsUtil.GetVisibleScreenSize(scene);
        this.Shader = SKShader.FromFile("Shaders/Glass.fsh");

        Vector2 resolution = new Vector2((float)screenSize.Width, (float)screenSize.Height);
        this.Shader.Uniforms = new SKUniform[] {
         new SKUniform("i_resolution", resolution)
        };
      }
      helpNode.ToggleState(false);
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
      scene.View.AddGestureRecognizer(swipeDownOverToolsGesture);
      
      swipeUpGesture.ShouldRequireFailureOf = (gesture, otherGesture) =>
        gesture == swipeUpGesture && (
              otherGesture == swipeUpOverToolsGesture ||
              otherGesture == swipeDownOverToolsGesture );

      tap.ShouldRequireFailureOf = (gesture, otherGesture) =>
        gesture == tap && (
              otherGesture == swipeUpOverToolsGesture ||
              otherGesture == swipeDownOverToolsGesture ||
              otherGesture == swipeUpGesture);

      pan.ShouldRequireFailureOf = (gesture, otherGesture) =>
        gesture == pan && (
              otherGesture == swipeUpGesture ||
              otherGesture == swipeUpOverToolsGesture ||
              otherGesture == swipeDownOverToolsGesture);
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
      scene.View.RemoveGestureRecognizer(swipeDownOverToolsGesture);
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
      Close();
      OnOk?.Invoke(this, GetSelectedButton()?.Name);
      var hud = Container.GetContainer().GetInstance<Hud>();
      hud.SetMessage(String.Empty);
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
      ClearSelection();
      var hud = Container.GetContainer().GetInstance<Hud>();
      hud.SetMessage(String.Empty);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gesture"></param>
    private void OnSwipeOverMenu(SwipeOverSpriteGestureRecognizer gesture)
    {
      var actualStep = 0;
      var step = gesture.GestureSize * 5;

      if(gesture.Direction == UISwipeGestureRecognizerDirection.Down) {
        if ((int)floatMenu.Position.Y + marginTop > this.Size.Height/2 ) {
          actualStep = -(int)Math.Min(step,
            Math.Abs(this.Size.Height/2 - (int)floatMenu.Position.Y - marginTop));
        }
      } else {
        if ((int)floatMenu.Position.Y - floatMenu.Size.Height < -this.Size.Height / 2) {
          actualStep = (int)Math.Min(step,
            Math.Abs(this.Size.Height / 2)- (int)floatMenu.Position.Y + floatMenu.Size.Height);
        }
      }

      var action = SKAction.MoveBy(0, actualStep, 0.5);
      var group = SKAction.Sequence(action );
      floatMenu.RunAction(group);
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
        this.Size.Height / 2 - marginTop);

      helpNode.Position = new CGPoint(
        this.Size.Width / 2,
        this.Size.Height / 2 - marginTop);
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
    public List<GridCoord> GetSelectedTiles() {
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
      var hud = Container.GetContainer().GetInstance<Hud>();
      hud.SetMessage(String.Empty);
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

        var key = selected.Keys.Where(x => x.Row == row && x.Col == col).FirstOrDefault();
        if (key != null) {
          var val = selected.Where(x => x.Key == key).First();
          selected.Remove(key);
          val.Value.RemoveFromParent();
        } else {
          if (SelectionValidator != null && !SelectionValidator(this, new GridCoord(row, col))) {
            hud.SetMessage("Cannot build there");
            return;
          }
          var square = HighlightTile(row, col, true);
          tileMap.GetLayer(tileMap.LayerCount - 1).AddChild(square);
          selected.Add(new GridCoord(row, col), square);
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
