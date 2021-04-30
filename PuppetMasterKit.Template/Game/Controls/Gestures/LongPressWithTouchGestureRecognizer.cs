using System;
using Foundation;
using UIKit;

namespace PuppetMasterKit.Template.Game.Controls.Gestures
{
  public class LongPressWithTouchGestureRecognizer : UILongPressGestureRecognizer
  {
    private NSSet touches;

    public LongPressWithTouchGestureRecognizer(Action<UILongPressGestureRecognizer> action) : base(action)
    {
      
    }

    public NSSet Touches { get => touches; set => touches = value; }

    public override bool ShouldReceive(UIEvent @event)
    {
      touches = @event.AllTouches;
      return base.ShouldReceive(@event);
    }
  }
}
