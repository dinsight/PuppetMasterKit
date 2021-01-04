using System;
using System.Collections.Generic;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.Graphics.Sprites
{
  public interface ISprite
  {
    float ZOrder { get; set; }

    float Alpha { get; set; }

    Point Position { get; set; }

    Point RelativePosition { get; set; }

    Size Size { get; set; }

    Point AnchorPoint { get; set; }

    void RemoveFromParent();

    void AddToScene();

    void AddChild(ISprite sprite);

    void AddProperty(String name, String value);

    String GetProperty(String name);

    object GetNativeSprite();

    void SetBorder();
  }
}
