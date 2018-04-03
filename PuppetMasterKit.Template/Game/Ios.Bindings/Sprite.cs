using System;
using System.Collections.Generic;
using Foundation;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;

namespace PuppetMasterKit.Template.Game.Ios.Bindings
{
  public class Sprite : ISprite
  {
    SKScene scene;

    SKSpriteNode node;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Template.Game.Ios.Bindings.Sprite"/> class.
    /// </summary>
    /// <param name="node">Node.</param>
    /// <param name="scene">Scene.</param>
    public Sprite(SKSpriteNode node, SKScene scene)
    {
      this.scene = scene;
      this.node = node;
    }

    /// <summary>
    /// Gets or sets the alpha.
    /// </summary>
    /// <value>The alpha.</value>
    public float Alpha {
      get {
        return (float)node.Alpha;
      }
      set {
        node.Alpha = value;
      }
    }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>The position.</value>
    public Point Position {
      get {
        return
            new Point((float)node.Position.X, (float)node.Position.Y);
      }

      set {
        node.Position = new CoreGraphics.CGPoint(value.X, value.Y);
      }
    }

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>The size.</value>
    public Size Size {
      get {
        return
            new Size((float)node.Size.Width, (float)node.Size.Height);
      }

      set {
        node.Size = new CoreGraphics.CGSize(value.Width, value.Height);
      }
    }

    /// <summary>
    /// Gets or sets the anchor point.
    /// </summary>
    /// <value>The anchor point.</value>
    public Point AnchorPoint {
      get {
        return
            new Point((float)node.AnchorPoint.X, (float)node.AnchorPoint.Y);
      }

      set {
        node.AnchorPoint = new CoreGraphics.CGPoint(value.X, value.Y);
      }
    }

    /// <summary>
    /// Adds the child.
    /// </summary>
    /// <param name="sprite">Sprite.</param>
    public void AddChild(ISprite sprite)
    {
      var local = sprite as Sprite;
      node.AddChild(local.node);
    }

    /// <summary>
    /// Adds to scene.
    /// </summary>
    public void AddToScene()
    {
      scene.AddChild(node);
    }

    /// <summary>
    /// Adds the property.
    /// </summary>
    /// <param name="name">Name.</param>
    /// <param name="value">Value.</param>
    public void AddProperty(string name, String value)
    {
      node.UserData.Add(new NSString(name), NSObject.FromObject(value));
    }

    /// <summary>
    /// Gets the property.
    /// </summary>
    /// <returns>The property.</returns>
    /// <param name="name">Name.</param>
    public String GetProperty(string name)
    {
      NSObject val = null;
      node.UserData.TryGetValue(new NSString(name), out val);
      return val.ToString();
    }

    /// <summary>
    /// Removes from parent.
    /// </summary>
    public void RemoveFromParent()
    {
      node.RemoveFromParent();
    }
  }
}
