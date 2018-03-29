using System;

using CoreGraphics;
using Foundation;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Template.Ios.Bindings;
using SpriteKit;
using UIKit;

namespace PuppetMasterKit.Template
{
    public class GameScene : SKScene
    {
        double prevTime = 0;

        ComponentSystem agentSystem = new ComponentSystem();

        ComponentSystem componentSystem = new ComponentSystem();

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

            var entity = new Entity()
                .With(componentSystem, 
                    new SpriteComponent<string>("rabbit", "r"),
                    new HealthComponent())
                .With(agentSystem,
                    new Agent());
        }

        /// <summary>
        /// Toucheses the began.
        /// </summary>
        /// <param name="touches">Touches.</param>
        /// <param name="evt">Evt.</param>
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
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
