using System;
using System.Linq;
using Foundation;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Template.Game;
using PuppetMasterKit.Template.Game.Character;
using PuppetMasterKit.Template.Game.Ios.Bindings;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template
{
  public class GameScene : SKScene
  {
    private double prevTime = 0;

    private FlightMap flightMap = new FlightMap(); 

    private ComponentSystem agentSystem = new ComponentSystem();

    private ComponentSystem componentSystem = new ComponentSystem();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Template.GameScene"/> class.
    /// </summary>
    /// <param name="handle">Handle.</param>
    protected GameScene(IntPtr handle) : base(handle)
    {
    }

    /// <summary>
    /// Dids the move to view.
    /// </summary>
    /// <param name="view">View.</param>
    public override void DidMoveToView(SKView view)
    {
      Registration.RegisterBindings(this);

      var rabbit = RabbitBuilder.Build(componentSystem);
      var theSprite = rabbit.GetComponent<SpriteComponent>()?.Sprite;
      theSprite.Position = new Point(100, 100);
      flightMap.Add(rabbit);
    }

    /// <summary>
    /// Toucheses the began.
    /// </summary>
    /// <param name="touches">Touches.</param>
    /// <param name="evt">Evt.</param>
    public override void TouchesBegan(NSSet touches, UIEvent evt)
    {
      foreach (UITouch touch in touches) {
        var positionInScene = touch.LocationInNode(this);
        var touchedNode = this.GetNodeAtPoint(positionInScene);
        if (touchedNode.UserData != null) {
          ToggleSelection(touchedNode);
        }
      }
    }

    /// <summary>
    /// Toggles the selection.
    /// </summary>
    /// <param name="touchedNode">Touched node.</param>
    private void ToggleSelection(SKNode touchedNode)
    {
      var id = touchedNode.UserData[SpriteComponent.ENTITY_ID_PPROPERTY];
      if (id != null) {
        var entity = flightMap.GetEntityById(id.ToString());
        var state = entity.GetComponent<StateComponent>();
        state.IsSelected = !state.IsSelected;
      }
    }

    /// <summary>
    /// Update the specified currentTime.
    /// </summary>
    /// <returns>The update.</returns>
    /// <param name="currentTime">Current time.</param>
    public override void Update(double currentTime)
    {
      var delta = currentTime - prevTime;
      prevTime = currentTime;
      agentSystem.Update(delta);
      componentSystem.Update(delta);
      base.Update(currentTime);
    }
  }
}
