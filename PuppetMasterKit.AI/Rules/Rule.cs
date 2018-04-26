using System;
namespace PuppetMasterKit.AI.Rules
{
  public class Rule<S>
  {
    private Func<Entity, S, FactSet, bool> predicate;

    private Action<Entity, S, FactSet> action;

    internal bool IsExecuted { get; set;}

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Inference.Rule`1"/> class.
    /// </summary>
    /// <param name="predicate">Predicate.</param>
    /// <param name="action">Action.</param>
    public Rule(Func<Entity, S, FactSet, bool> predicate, Action<Entity, S, FactSet> action)
    {
      this.predicate = predicate;
      this.action = action;
      this.IsExecuted = false;
    }

    /// <summary>
    /// Evaluate the specified state and factSet.
    /// </summary>
    /// <returns>The evaluate.</returns>
    /// <param name="state">State.</param>
    /// <param name="factSet">Fact set.</param>
    public void Evaluate(Entity entity,S state, FactSet factSet)
    {
      if (predicate(entity, state, factSet)) {
        action(entity, state, factSet);
        IsExecuted = true;
      }
    }
  }
}
