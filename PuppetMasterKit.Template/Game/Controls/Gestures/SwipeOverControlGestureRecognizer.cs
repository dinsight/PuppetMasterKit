using System;
using System.Linq;
using System.Linq.Expressions;
using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template.Game.Controls.Gestures
{
  public class SwipeOverSpriteGestureRecognizer: UISwipeGestureRecognizer
  {
    private int minGestureSize = 15;
    private SKSpriteNode target;
    private CGPoint startLocation;
    private double gestureSize;
    private Action<SwipeOverSpriteGestureRecognizer> action;

    /// <summary>
    /// 
    /// </summary>
    public int GestureSize { get => (int)gestureSize; }
    public int MinGestureSize { get => minGestureSize; set => minGestureSize = value; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="action"></param>
    public SwipeOverSpriteGestureRecognizer(SKSpriteNode target,
      Action<SwipeOverSpriteGestureRecognizer> action) 
    {
      this.target = target;
      this.action = action;
    }

    public override void TouchesBegan(NSSet touches, UIEvent evt)
    {
      startLocation = touches.Cast<UITouch>().First().LocationInView(this.View);
      base.TouchesBegan(touches, evt);
    }

    public override void TouchesMoved(NSSet touches, UIEvent evt)
    {
      var touch = touches.Cast<UITouch>().First();
      var currentLocation = touch.LocationInView(this.View);
      var vertDiff = startLocation.Y - currentLocation.Y;

      gestureSize = Math.Abs(vertDiff);
      if (gestureSize >= minGestureSize)
      {
        var nodePos = touch.LocationInNode(target.Parent);
        if (target.ContainsPoint(nodePos)) {
          action?.Invoke(this);
          State = UIGestureRecognizerState.Ended;
        } else {
          State = UIGestureRecognizerState.Failed;
        }
      }
      
      base.TouchesMoved(touches, evt);
    }

    public override void TouchesEnded(NSSet touches, UIEvent evt)
    {
      Reset();
    }

    public override void TouchesCancelled(NSSet touches, UIEvent evt)
    {
      Reset();
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Reset() {
      gestureSize = 0;
      startLocation = new CGPoint(0, 0);
      if (State == UIGestureRecognizerState.Possible) {
        State = UIGestureRecognizerState.Failed;
      }
      base.Reset();
    }
  }
}
