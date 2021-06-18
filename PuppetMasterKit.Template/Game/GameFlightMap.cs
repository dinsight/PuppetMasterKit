using System;
using System.Collections.Generic;
using System.Linq;
using PuppetMasterKit.AI;
using PuppetMasterKit.Terrain.Map;
using PuppetMasterKit.Utility.Extensions;

namespace PuppetMasterKit.Template.Game
{
  public class GameFlightMap : FlightMap
  {
    private List<Entity> heroes = new List<Entity>();

    private Dictionary<String, int> heroPoints = new Dictionary<String, int>();

    public List<Obstacle> Obstacles { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="tileSize"></param>
    /// <param name="partitionsCountX"></param>
    /// <param name="partitionsCountY"></param>
    public GameFlightMap(int[,] map, int tileSize,
                         int partitionsCountX = 10,
                         int partitionsCountY = 10)
      : base(map,
          map.GetLength(0),
          map.GetLength(1),
          map.GetLength(0) * tileSize,
          map.GetLength(1) * tileSize,
          partitionsCountX,
          partitionsCountY)
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
