using System;
using LightInject;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.AI.Components
{
  public class HealthComponent : Component
  {
    public float MaxHealth { get; set; }

    public float Width { get; private set; }

    public float Height { get; private set; }

    public Color FillColor { get; set; }

    public Color DamageColor { get; set; }

    private ISprite healthSprite;

    private ISprite damageSprite;

    private float health;

    /// <summary>
    /// Gets or sets the health.
    /// </summary>
    /// <value>The health.</value>
    public float Damage {
      get { return MaxHealth - health; }
      set {
        health = Math.Max(0, MaxHealth - value);
        var val = Math.Min(MaxHealth, value);
        var damage = val * Width / MaxHealth;
        damageSprite.Size = new Size(damage, damageSprite.Size.Height);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Components.HealthComponent"/> class.
    /// </summary>
    /// <param name="health">Health.</param>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    public HealthComponent(float health, float width = 100, float height = 8)
    {
      this.health = health;
      this.MaxHealth = health;
      this.Width = width;
      this.Height = height;
      this.FillColor = new Color(0, 1, 0, 1);
      this.DamageColor = new Color(1, 0, 0, 1);
    }

    /// <summary>
    /// Creates the sprite.
    /// </summary>
    private void CreateSprite()
    {
      var spriteComponent = Entity.GetComponent<SpriteComponent>();
      var factory = Container.GetContainer().GetInstance<ISpriteFactory>();
      healthSprite = factory.CreateSprite(FillColor.Red, FillColor.Green, FillColor.Blue, FillColor.Alpha);
      damageSprite = factory.CreateSprite(DamageColor.Red, DamageColor.Green, DamageColor.Blue, DamageColor.Alpha);
      healthSprite.Size = new Size(Width, Height);
      damageSprite.Size = new Size(0, Height);
      healthSprite.RelativePosition = new Point(0, spriteComponent.Sprite.Size.Height);
      healthSprite.AddChild(damageSprite);
      damageSprite.RelativePosition = new Point(Width / 2, 0);
      damageSprite.AnchorPoint = new Point(0f, 0.5f);
      spriteComponent.Sprite.AddChild(healthSprite);
    }

    /// <summary>
    /// Ons the set entity.
    /// </summary>
    public override void OnSetEntity()
    {
      CreateSprite();
      base.OnSetEntity();
    }

    /// <summary>
    /// Cleanup this instance.
    /// </summary>
    public override void Cleanup()
    {
      healthSprite.RemoveFromParent();
      healthSprite = null;
      base.Cleanup();
    }
  }
}
