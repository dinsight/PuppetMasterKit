using System;
using PuppetMasterKit.AI.Configuration;
using Foundation;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using LightInject;
using SpriteKit;
using CoreGraphics;

namespace PuppetMasterKit.Template.Game.Ios.Bindings
{
  public class Sprite : ISprite
  {
    SKScene scene;

    SKSpriteNode node;

    ICoordinateMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Template.Game.Ios.Bindings.Sprite"/> class.
    /// </summary>
    /// <param name="node">Node.</param>
    /// <param name="scene">Scene.</param>
    public Sprite(SKSpriteNode node, SKScene scene)
    {
      this.scene = scene;
      this.node = node;
      this.mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
    }

    /// <summary>
    /// Adds the debug border.
    /// </summary>
    private void AddDebugBorder(){
      var frame = CGRect.FromLTRB(0, node.Size.Height, node.Size.Width, 0);

      var border = SKShapeNode.FromRect(frame);
      node.AddChild(border);
      border.StrokeColor = UIKit.UIColor.Red;
      border.LineWidth = 2;
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
        return mapper.FromScene(
          new Point((float)node.Position.X, (float)node.Position.Y));
      }

      set {
        if (!node.Position.X.Equals(value.X) ||
            !node.Position.Y.Equals(value.Y)) {
          var translated = mapper.ToScene(value);
          node.Position = new CoreGraphics.CGPoint(translated.X, translated.Y);
        }
      }
    }

    /// <summary>
    /// Gets or sets the relative position.
    /// </summary>
    /// <value>The relative position.</value>
    public Point RelativePosition { 
      get{
        return new Point((float)node.Position.X, (float)node.Position.Y);
      }

      set{
        if (!node.Position.X.Equals(value.X) ||
            !node.Position.Y.Equals(value.Y)) { 
          node.Position = new CoreGraphics.CGPoint(value.X, value.Y);
        }
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
        //AddDebugBorder();
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

    /// <summary>
    /// Gets the native sprite.
    /// </summary>
    /// <returns>The native sprite.</returns>
    public object GetNativeSprite()
    {
      return node;
    }
  }
}
