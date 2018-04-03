using System;
using System.Linq;
using Foundation;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Template.Game;
using PuppetMasterKit.Template.Game.Character;
using PuppetMasterKit.Template.Game.Ios.Bindings;
using PuppetMasterKit.Utility;
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
      {
        var rabbit = RabbitBuilder.Build(componentSystem);
        var theSprite = rabbit.GetComponent<SpriteComponent>()?.Sprite;
        theSprite.Position = new Point(100, 100);
        flightMap.Add(rabbit);
      }
      {
        var rabbit = RabbitBuilder.Build(componentSystem);
        var theSprite = rabbit.GetComponent<SpriteComponent>()?.Sprite;
        theSprite.Position = new Point(100, 300);
        flightMap.Add(rabbit);
      }
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
        var entity = GetEntityFromNode(touchedNode);
        if (entity != null) {
          OnEntitySelected(entity);
        } else {
          OnNodeSelected(touchedNode);
        }
      }
    }

    /// <summary>
    /// Gets the entity from node.
    /// </summary>
    /// <returns>The entity from node.</returns>
    /// <param name="node">Node.</param>
    private Entity GetEntityFromNode(SKNode node)
    {
      if (node.UserData == null)
        return null;
                 
      var id = node.UserData[SpriteComponent.ENTITY_ID_PPROPERTY];
      if (id != null) { 
        return flightMap.GetEntityById(id.ToString());
      }
      return null;
    }

    /// <summary>
    /// On selected entity.
    /// </summary>
    /// <param name="entity">Entity.</param>
    private void OnEntitySelected(Entity entity)
    {
      if (entity == null)
        return;

      var state = entity.GetComponent<StateComponent>();
      var touch = entity.GetComponent<TouchComponent>();

      if (state!=null) {
        state.IsSelected = !state.IsSelected;
      }
      if (touch!=null) {
        touch.OnTargetTouched(entity);
      }
    }

    /// <summary>
    /// On selected scene.
    /// </summary>
    /// <param name="touchedNode">Touched node.</param>
    private void OnNodeSelected(SKNode touchedNode)
    {
      Point point = new Point((float)touchedNode.Position.X, 
                              (float)touchedNode.Position.Y);

      flightMap.GetEntities(x => {
        var state = x.GetComponent<StateComponent>();
        return state != null && state.IsSelected;
      })
      .ForEach(e=>e.GetComponent<TouchComponent>()?.OnSceneTouched(point));
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
