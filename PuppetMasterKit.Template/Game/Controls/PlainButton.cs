﻿using System;
using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template.Game.Controls
{
  public class PlainButton : HoverButton
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    public PlainButton(IntPtr handle) : base(handle)
    {
      neutralTexture = null;
      SetNormalTexture();
      this.UserInteractionEnabled = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="touches"></param>
    /// <param name="evt"></param>
    public override void TouchesBegan(NSSet touches, UIEvent evt)
    {
      base.TouchesBegan(touches, evt);
      NotifyButtonPressed(this, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="touches"></param>
    /// <param name="evt"></param>
    public override void TouchesEnded(NSSet touches, UIEvent evt)
    {
      base.TouchesEnded(touches, evt);
      NotifyButtonReleased(this, null);
    }
  }
}