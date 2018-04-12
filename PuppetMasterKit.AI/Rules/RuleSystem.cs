﻿using System;
using System.Collections.Generic;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.AI.Rules
{
  public class RuleSystem<T>
  {
    private T state;

    private List<Rule<T>> rules = new List<Rule<T>>();

    private FactSet Facts { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Inference.RuleSystem`1"/> class.
    /// </summary>
    /// <param name="state">State.</param>
    public RuleSystem(T state)
    {
      Facts = new FactSet();
      this.state = state;
    }

    /// <summary>
    /// Add the specified rule.
    /// </summary>
    /// <returns>The add.</returns>
    /// <param name="rule">Rule.</param>
    public void Add(Rule<T> rule)
    {
      rules.Add(rule);
    }

    /// <summary>
    /// Removes all.
    /// </summary>
    public void RemoveAll()
    {
      rules.Clear();
    }

    /// <summary>
    /// Reset this instance.
    /// </summary>
    public void Reset()
    {
      Facts.Clear();
      rules.ForEach(x => x.IsExecuted = false);
    }

    /// <summary>
    /// Evaluate this instance.
    /// </summary>
    public Fact Evaluate()
    {
      do {
        Facts.IsDirty = false;
        rules.ForEach(x=>x.Evaluate(state, Facts));
      } while (Facts.IsDirty);
      return Facts.Max();
    }
  }
}
