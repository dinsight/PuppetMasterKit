using System;
using Foundation;
using UIKit;

namespace PuppetMasterKit.Template.Game.Controls.Gestures
{
  public class TapWithTouchGestureRecognizer : UITapGestureRecognizer
  {
    private NSSet touches;

    public TapWithTouchGestureRecognizer(Action<UITapGestureRecognizer> action) : base(action)
    {
      this.CancelsTouchesInView = true;
    }

    public NSSet Touches { get => touches; set => touches = value; }

    public override bool ShouldReceive(UIEvent @event)
    {
      touches = @event.AllTouches;
      return base.ShouldReceive(@event);
    }
  }
}
