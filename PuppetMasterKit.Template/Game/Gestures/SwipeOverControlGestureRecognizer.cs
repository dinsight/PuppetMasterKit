using System;
using System.Linq;
using System.Linq.Expressions;
using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template.Game.Gestures
{
  public class SwipeOverControlGestureRecognizer: UISwipeGestureRecognizer
  {
    private int minGestureSize = 15;
    private SKSpriteNode target;
    private CGPoint startLocation;
    private Action<SwipeOverControlGestureRecognizer> action;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="action"></param>
    public SwipeOverControlGestureRecognizer(SKSpriteNode target,
      Action<SwipeOverControlGestureRecognizer> action) 
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
      //swipe up
      if (vertDiff > 0) {
        var gestureSize = Math.Abs(vertDiff);
        if (gestureSize >= minGestureSize)
        {
          var nodePos = touch.LocationInNode(target.Parent);
          if (target.ContainsPoint(nodePos)) {
            action?.Invoke(this);
            State = UIGestureRecognizerState.Ended;
          }
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
      startLocation = new CGPoint(0, 0);
      if (State == UIGestureRecognizerState.Possible) {
        State = UIGestureRecognizerState.Failed;
      }
      base.Reset();
    }
  }
}
