using System;
using System.Linq;
using System.Collections.Generic;

namespace PuppetMasterKit.AI.Rules
{
  public class FactSet
  {
    private Dictionary<int, Fact> facts = new Dictionary<int, Fact>();

    internal bool IsDirty { get; set; }

    /// <summary>
    /// Gets the count.
    /// </summary>
    /// <value>The count.</value>
    public int Count { get {
        return facts.Count;
      }
    }

    /// <summary>
    /// Assert the specified fact.
    /// </summary>
    /// <returns>The assert.</returns>
    /// <param name="fact">Fact.</param>
    public Fact Assert(Fact fact)
    {
      if (facts.ContainsKey(fact.GetHashCode())) {
        facts[fact.GetHashCode()].Grade = fact.Grade;
      } else {
        facts.Add(fact.GetHashCode(), fact);
      }
      IsDirty = true;
      return fact;
    }

    /// <summary>
    /// Retract the specified grade.
    /// </summary>
    /// <returns>The retract.</returns>
    /// <param name="grade">Grade.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public void Retract<T>(float grade =1) where T : Fact
    {
      var fact = GetFact<T>();
      if (fact == null)
        return;
      
      if(fact.Grade - grade <=0 ){
        facts.Remove(fact.GetHashCode());
      } else {
        fact.Grade -= grade;
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

    /// <summary>
    /// Zadeh AND operator
    /// </summary>
    /// <returns>The minimum.</returns>
    public Fact Min()
    {
      return facts.Values.OrderBy(x => x.Grade).FirstOrDefault();
    }

    /// <summary>
    /// Zadeh OR operator
    /// </summary>
    /// <returns>The max.</returns>
    public Fact Max()
    {
      return facts.Values.OrderByDescending(x => x.Grade).FirstOrDefault();
    }
  }
}
