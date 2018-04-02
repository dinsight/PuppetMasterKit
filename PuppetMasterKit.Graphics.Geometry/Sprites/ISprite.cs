﻿using System;
using System.Collections.Generic;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.Graphics.Sprites
{
  public interface ISprite
  {
    float Alpha { get; set; }

    Point Position { get; set; }

    Size Size { get; set; }

    Point AnchorPoint { get; set; }

    void RemoveFromParent();

    void AddToScene();

    void AddChild(ISprite sprite);

    void AddProperty(String name, Object value);

    Object GetProperty(String name);
  }
}
