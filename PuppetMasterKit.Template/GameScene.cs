using System;
using System.Security.Cryptography;
using CoreGraphics;
using Foundation;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Template.Game;
using PuppetMasterKit.Template.Game.Character.Rabbit;
using PuppetMasterKit.Template.Game.Character.Wolf;
using PuppetMasterKit.Template.Game.Ios.Bindings;
using PuppetMasterKit.Utility;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template
{
  public class GameScene : SKScene
  {
    private bool isMultiSelect = false;

    private double prevTime = 0;

    private FlightMap flightMap;

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
      flightMap = new FlightMap((float)view.Frame.Width, (float)view.Frame.Height, 7, 7);
      Registration.RegisterBindings(this);
      Registration.Register(flightMap);

      for (int i = 0; i < 1 ; i++) {
        var rabbit = RabbitBuilder.Build(componentSystem);
        var theSprite = rabbit.GetComponent<SpriteComponent>()?.Sprite;
        var random = new Random(Guid.NewGuid().GetHashCode());
        var x = random.Next(10, 300);
        var y = random.Next(100, 600);
        theSprite.Position = new Point(x, y);
        flightMap.Add(rabbit);
      }

      for (int i = 0; i < 1; i++) {
        var wolf = WolfBuilder.Build(componentSystem);
        var theSprite = wolf.GetComponent<SpriteComponent>()?.Sprite;
        var random = new Random(Guid.NewGuid().GetHashCode());
        var x = random.Next(10, 300);
        var y = random.Next(100, 600);
        theSprite.Position = new Point(x, y);
        flightMap.Add(wolf);
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
          OnEntityTouched(entity);
        } else {
          OnSceneTouched(positionInScene);
        }
      }
    }

    /// <summary>
    /// On selected entity.
    /// </summary>
    /// <param name="entity">Entity.</param>
    private void OnEntityTouched(Entity entity)
    {
      if (entity == null)
        return;

      var touch = entity.GetComponent<CommandComponent>();
      if (!isMultiSelect) {
        //if multisect is disabled, clear existing selections
        flightMap.GetEntities(x => {
          var state = x.GetComponent<StateComponent>();
          return state != null && state.IsSelected;
        }).ForEach(x => {
          var state = x.GetComponent<StateComponent>();
          state.IsSelected = false;
        });
      }
      //send touched event to the entity's command component
      if (touch != null) {
        touch.OnTouched(entity);
      }
    }

    /// <summary>
    /// On selected scene.
    /// </summary>
    /// <param name="location">Touched node.</param>
    private void OnSceneTouched(CGPoint location)
    {
      Point point = new Point((float)location.X, (float)location.Y);

      flightMap.GetEntities(x => {
        var state = x.GetComponent<StateComponent>();
        return state != null && state.IsSelected;
      })
      .ForEach(e => e.GetComponent<CommandComponent>()?
               .OnMoveToPoint(e, point));
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
    /// Update the specified currentTime.
    /// </summary>
    /// <returns>The update.</returns>
    /// <param name="currentTime">Current time.</param>
    public override void Update(double currentTime)
    {
      var delta = (float)(currentTime - prevTime);
      prevTime = currentTime;
      agentSystem.Update(delta);
      componentSystem.Update(delta);
      base.Update(currentTime);
    }
  }
}
