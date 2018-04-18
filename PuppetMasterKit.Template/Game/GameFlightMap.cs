using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.AI;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.Template.Game
{
  public class GameFlightMap : FlightMap
  {
    private List<Entity> heroes = new List<Entity>();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Template.Game.GameFlightMap"/> class.
    /// </summary>
    /// <param name="mapWidth">Map width.</param>
    /// <param name="mapHeight">Map height.</param>
    /// <param name="partitionsCountX">Partitions count x.</param>
    /// <param name="partitionsCountY">Partitions count y.</param>
    public GameFlightMap(float mapWidth, 
                         float mapHeight,
                         int partitionsCountX = 10,
                         int partitionsCountY = 10) 
      : base(mapWidth, mapHeight, partitionsCountX, partitionsCountY)
    {
      
    }

    /// <summary>
    /// Adds the hero.
    /// </summary>
    /// <param name="hero">Hero.</param>
    public void AddHero(Entity hero)
    {
      heroes.Add(hero);
      base.Add(hero);
    }

    /// <summary>
    /// Gets the heroes.
    /// </summary>
    /// <returns>The heroes.</returns>
    public Entity[] GetHeroes()
    {
      return heroes.ToArray();
    }

    /// <summary>
    /// Remove the specified entity.
    /// </summary>
    /// <returns>The remove.</returns>
    /// <param name="entity">Entity.</param>
    public override void Remove(Entity entity)
    {
      base.Remove(entity);
      var toRemove = heroes.Where(x => x.Id == entity.Id).ToArray();
      toRemove.ForEach(x=>{
        heroes.Remove(x);
      });
    }
  }
}
