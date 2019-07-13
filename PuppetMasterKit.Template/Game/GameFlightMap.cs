using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.AI;
using PuppetMasterKit.Utility.Extensions;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.Template.Game
{
  public class GameFlightMap : FlightMap
  {
    private List<Entity> heroes = new List<Entity>();

    private Dictionary<String, int> heroPoints = new Dictionary<string, int>();

    public List<Obstacle> Obstacles { get; set; }

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
      Obstacles = new List<Obstacle>();
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
    /// Gets the score.
    /// </summary>
    /// <returns>The score.</returns>
    /// <param name="id">Identifier.</param>
    public int GetScore(string id)
    {
      if (heroPoints.ContainsKey(id)) {
        return heroPoints[id];
      }
      return 0;
    }

    /// <summary>
    /// Sets the score.
    /// </summary>
    /// <param name="id">Identifier.</param>
    /// <param name="value">Value.</param>
    public void SetScore(string id, int value)
    {
      if(heroPoints.ContainsKey(id)){
        heroPoints[id] = value;
      } else {
        heroPoints.Add(id, value);
      }
    }

    /// <summary>
    /// Adds to score.
    /// </summary>
    /// <param name="id">Identifier.</param>
    /// <param name="value">Value.</param>
    public void AddToScore(string id, int value)
    {
      if (heroPoints.ContainsKey(id)) {
        heroPoints[id] += value;
      } else {
        heroPoints.Add(id, value);
      }
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

    /// <summary>
    /// Gets the obstacles.
    /// </summary>
    /// <returns>The obstacles.</returns>
    /// <param name="entity">Entity.</param>
    public IEnumerable<Obstacle> GetObstacles(Entity entity)
    {
      return Obstacles;
    }
  }
}
