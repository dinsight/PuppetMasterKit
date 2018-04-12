using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using PuppetMasterKit.AI.Rules;

namespace PuppetMasterKit.AI.Components
{
  public class RuleSystemComponent<T,F> : Component where F: FactHandler
  {
    private RuleSystem<T> ruleSystem;

    private FactHandler factHandler;

    private bool canEvaluate = true;

    private TimeSpan delayEvaluation;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Components.RuleSystemComponent`2"/> class.
    /// </summary>
    /// <param name="ruleSystem">Rule system.</param>
    /// <param name="factHandler">Fact handler.</param>
    public RuleSystemComponent(RuleSystem<T> ruleSystem, 
                               F factHandler): 
        this(ruleSystem, factHandler, TimeSpan.FromSeconds(15))
    {
      
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Components.RuleSystemComponent`2"/> class.
    /// </summary>
    /// <param name="ruleSystem">Rule system.</param>
    /// <param name="factHandler">Fact handler.</param>
    /// <param name="delayEvaluation">Delay evaluation.</param>
    public RuleSystemComponent(RuleSystem<T> ruleSystem, 
                               F factHandler, 
                               TimeSpan delayEvaluation)
    {
      this.factHandler = factHandler;
      this.ruleSystem = ruleSystem;
      this.delayEvaluation = delayEvaluation;
    }

    /// <summary>
    /// Evaluate this instance.
    /// </summary>
    private async Task<bool> Evaluate()
    {
      await Task.Delay(delayEvaluation);
      Fact fact = ruleSystem.Evaluate();
      if (!(fact is null)) {
        dynamic handler = factHandler;
        handler.Handle(fact as dynamic);
      }
      canEvaluate = true;
      return canEvaluate;
    }

    /// <summary>
    /// Update the specified deltaTime.
    /// </summary>
    /// <returns>The update.</returns>
    /// <param name="deltaTime">Delta time.</param>
    public override void Update(double deltaTime)
    {
      if(canEvaluate){
        canEvaluate = false;
        Task.Factory.StartNew(() => Evaluate());
      }
      base.Update(deltaTime);
    }
  }
}
