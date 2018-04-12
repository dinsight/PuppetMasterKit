using System;
namespace PuppetMasterKit.AI.Rules
{
  public class Rule<T>
  {
    private Predicate<T> predicate;

    private Action<T,FactSet> action;

    internal bool IsExecuted { get; set;}

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Inference.Rule`1"/> class.
    /// </summary>
    /// <param name="predicate">Predicate.</param>
    /// <param name="action">Action.</param>
    public Rule(Predicate<T> predicate, Action<T,FactSet> action)
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
    public void Evaluate(T state, FactSet factSet)
    {
      if (!IsExecuted && predicate(state)) {
        action(state, factSet);
        IsExecuted = true;
      }
    }
  }
}
