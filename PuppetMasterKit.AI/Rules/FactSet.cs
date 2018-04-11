using System;
using System.Linq;
using System.Collections.Generic;

namespace PuppetMasterKit.AI.Rules
{
  internal class FactSet
  {
    private Dictionary<int, Fact> facts = new Dictionary<int, Fact>();

    internal bool IsDirty { get; set; }

    /// <summary>
    /// Assert the specified fact.
    /// </summary>
    /// <returns>The assert.</returns>
    /// <param name="fact">Fact.</param>
    public Fact Assert(Fact fact)
    {
      facts.Add(fact.GetHashCode(), fact);
      IsDirty = true;
      return fact;
    }

    /// <summary>
    /// Retract the specified fact and grade.
    /// </summary>
    /// <returns>The retract.</returns>
    /// <param name="fact">Fact.</param>
    /// <param name="grade">Grade.</param>
    public void Retract(Fact fact, float grade = 1)
    {
      if(facts.ContainsKey(fact.GetHashCode())){
        var existing = facts[fact.GetHashCode()];
        if(existing.Grade - grade <= 0){
          facts.Remove(fact.GetHashCode());
        } else {
          existing.Grade -= grade;
        }
      }
    }

    /// <summary>
    /// Get this instance.
    /// </summary>
    /// <returns>The get.</returns>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public T GetFact<T>() where T : Fact
    {
      return facts.Where(x => x.Value is T)
                  .Select(a=>a.Value)
                  .FirstOrDefault() as T;
    }

    /// <summary>
    /// Gets the facts.
    /// </summary>
    /// <returns>The facts.</returns>
    public IEnumerable<Fact> GetFacts()
    {
      return facts.Values;
    }

    /// <summary>
    /// Clear this instance.
    /// </summary>
    public void Clear()
    {
      facts.Clear();
    }
  }
}
