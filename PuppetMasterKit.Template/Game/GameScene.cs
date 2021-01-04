using System;
using System.Diagnostics;
using System.Linq;
using CoreGraphics;
using Foundation;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Template.Game.Level;
using PuppetMasterKit.Utility.Extensions;
using SpriteKit;
using UIKit;
using LightInject;
using PuppetMasterKit.Graphics.Sprites;

namespace PuppetMasterKit.Template.Game
{
  public class GameScene : SKScene
  {
    private bool isMultiSelect = false;

    private double prevTime = 0;

    private GameFlightMap flightMap;

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
      flightMap = new LevelBuilder(this, componentSystem).Build() as GameFlightMap;
      this.View.MultipleTouchEnabled = true;
      flightMap.GetHeroes().ForEach(x => {
        x.GetComponent<StateComponent>().IsSelected = true; });
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
        OnTouchLocation(positionInScene);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="location"></param>
    private void OnTouchLocation(CGPoint location)
    {
      Point point = new Point((float)location.X, (float)location.Y);
      flightMap.GetHeroes()
        .Where(x => x.GetComponent<StateComponent>().IsSelected)
        .ForEach(c => {
          var cmd = c.GetComponent<CommandComponent>();
          cmd?.OnUserTouchedPoint(c, point);
        });
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
      componentSystem.CleanupOrphanComponents();
      flightMap.CleanupUnusedEntities();
      var delta = (float)(currentTime - prevTime);
      prevTime = currentTime;
      componentSystem.Update(delta);
      base.Update(currentTime);
    }
  }
}
