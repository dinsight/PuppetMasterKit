using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using PuppetMasterKit.AI;

namespace PuppetMasterKit.Template.Game
{
  public class FlightMap
  {
    private ConcurrentDictionary<string,Entity> Entities { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Template.Game.FlightMap"/> class.
    /// </summary>
    public FlightMap()
    {
      Entities = new ConcurrentDictionary<String,Entity>();
    }

    /// <summary>
    /// Gets the entity by identifier.
    /// </summary>
    /// <returns>The by identifier.</returns>
    /// <param name="id">Identifier.</param>
    public Entity GetEntityById(String id)
    {
      return Entities.GetValueOrDefault(id);
    }

    /// <summary>
    /// Add the specified entity.
    /// </summary>
    /// <returns>The add.</returns>
    /// <param name="entity">Entity.</param>
    public void Add(Entity entity)
    {
      Entities.TryAdd(entity.Id, entity);
    }

    /// <summary>
    /// Remove the specified entity.
    /// </summary>
    /// <returns>The remove.</returns>
    /// <param name="entity">Entity.</param>
    public void Remove(Entity entity)
    {
      Entity temp;
      Entities.TryRemove(entity.Id, out temp);
    }

    /// <summary>
    /// Gets the rabbits.
    /// </summary>
    /// <returns>The rabbits.</returns>
    /// <param name="requester">Requester.</param>
    public ICollection<Entity> GetRabbits(Entity requester)
    {
      return new List<Entity>(Entities.Values);
    }
  }
}
