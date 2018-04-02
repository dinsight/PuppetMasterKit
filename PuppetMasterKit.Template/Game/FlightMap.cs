using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using PuppetMasterKit.AI;

namespace PuppetMasterKit.Template.Game
{
  public class FlightMap
  {
    public BlockingCollection<Entity> Entities { get; private set; }

    public FlightMap()
    {
      Entities = new BlockingCollection<Entity>();
    }

    public ICollection<Entity> GetRabbits(Entity requester)
    {
      return new List<Entity>(Entities);
    }
  }
}
