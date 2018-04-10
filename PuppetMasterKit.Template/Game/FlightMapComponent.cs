using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;

namespace PuppetMasterKit.Template.Game
{
  public class FlightMapComponent : Component, IAgentDelegate
  {
    private FlightMap flightMap;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Template.Game.FlightMapComponent"/> class.
    /// </summary>
    /// <param name="flightMap">Flight map.</param>
    public FlightMapComponent(FlightMap flightMap)
    {
      this.flightMap = flightMap;
    }

    public void AgentDidUpdate(Agent agent)
    {
      flightMap.AgentDidUpdate(agent);
    }

    /// <summary>
    /// Agents the will update.
    /// </summary>
    /// <param name="agent">Agent.</param>
    public void AgentWillUpdate(Agent agent)
    {
      flightMap.AgentWillUpdate(agent);
    }
  }
}
