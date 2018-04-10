using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.AI.Components;

namespace PuppetMasterKit.AI
{
  public class FlightMap : IAgentDelegate
  {
    public EntityBucket[,] Buckets { get; private set; }

    private Dictionary<string, Entity> Entities { get; set; }

    private float mapWidth;

    private float mapHeight;

    private float partitionWidth;

    private float partitionHeight;

    private int partitionsCountX;

    private int partitionsCountY;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.FlightMap"/> class.
    /// </summary>
    /// <param name="mapWidth">Map width.</param>
    /// <param name="mapHeight">Map height.</param>
    /// <param name="partitionsCountX">Partitions count x.</param>
    /// <param name="partitionsCountY">Partitions count y.</param>
    public FlightMap(float mapWidth, float mapHeight, 
                     int partitionsCountX = 10 , 
                     int partitionsCountY = 10)
    {
      this.mapWidth = mapWidth;
      this.mapHeight = mapHeight;
      this.partitionWidth = mapWidth / partitionsCountX;
      this.partitionHeight = mapHeight / partitionsCountY;
      this.partitionsCountX = partitionsCountX;
      this.partitionsCountY = partitionsCountY;
      Entities = new Dictionary<String, Entity>();
      Buckets = new EntityBucket[partitionsCountX,partitionsCountY];
      //initialize the partitions
      for (int i = 0; i < partitionsCountX; i++) {
        for (int j = 0; j < partitionsCountY; j++) {
          Buckets[i, j] = new EntityBucket() {
            BucketId = new EntityBucketId(i, j)
          };
        }
      }
    }

    /// <summary>
    /// Gets the entity by identifier.
    /// </summary>
    /// <returns>The by identifier.</returns>
    /// <param name="id">Identifier.</param>
    public Entity GetEntityById(String id)
    {
      Entity temp;
      Entities.TryGetValue(id, out temp);
      return temp;
    }

    /// <summary>
    /// Add the specified entity.
    /// </summary>
    /// <returns>The add.</returns>
    /// <param name="entity">Entity.</param>
    public void Add(Entity entity)
    {
      var position = entity.GetComponent<Agent>().Position ?? Point.Zero;
      var quadrant = GetPartition(position);
      var bucket = Buckets[quadrant.Item1, quadrant.Item2];
      entity.BucketId = bucket.BucketId;
      lock (this) {
        Entities.Add(entity.Id, entity);
        bucket.Entitites.Add(entity.Id, entity);
      }
    }

    /// <summary>
    /// Remove the specified entity.
    /// </summary>
    /// <returns>The remove.</returns>
    /// <param name="entity">Entity.</param>
    public void Remove(Entity entity)
    {
      if(entity.BucketId != null){
        var bucket = GetBucket(entity.BucketId);
        lock(this){
          bucket.Entitites.Remove(entity.Id);
          Entities.Remove(entity.Id);
        }
      } else {
        lock(this){
          Entities.Remove(entity.Id);
        }
      }
    }

    /// <summary>
    /// Gets the entities.
    /// </summary>
    /// <returns>The entities.</returns>
    /// <param name="predicate">Predicate.</param>
    public ICollection<Entity> GetEntities(Predicate<Entity> predicate)
    {
      return new List<Entity>(Entities.Values.Where(x => predicate(x)));
    }

    /// <summary>
    /// Gets the partition coordinates.
    /// </summary>
    /// <returns>The partiton coordinate.</returns>
    /// <param name="point">Point.</param>
    public Tuple<int,int> GetPartition(Point point)
    {
      var i = (int)(point.X / partitionWidth);
      var j = (int)(point.Y / partitionHeight);
      //try not to create a separate bucket for points on the
      //bottom and right edges of the map
      if (point.X >= mapWidth) i--;
      if (point.Y >= mapHeight) j--;
      return new Tuple<int, int>(i, j);
    }


    /// <summary>
    /// Gets the bucket.
    /// </summary>
    /// <returns>The bucket.</returns>
    /// <param name="id">Id.</param>
    public EntityBucket GetBucket(EntityBucketId id)
    {
      if (id.Key1 <0 || id.Key2 < 0 
          || id.Key1 >= partitionsCountX 
          || id.Key2 >= partitionsCountY ) {
        return null;
      }
      var bucket = this.Buckets[id.Key1, id.Key2];

      return bucket;
    }

    /// <summary>
    /// Reallocate the specified entity.
    /// </summary>
    /// <returns>The reallocate.</returns>
    /// <param name="entity">Entity.</param>
    public void Reallocate(Entity entity)
    {
      if (entity.BucketId==null)
        return;
      
      var position = entity.GetComponent<Agent>().Position ?? Point.Zero;
      var quadrant = GetPartition(position);

      //Same quadrant. No need to change the bucket
      if (entity.BucketId.Key1 == quadrant.Item1 && 
          entity.BucketId.Key2 == quadrant.Item2)
        return;

      var newBucket = Buckets[quadrant.Item1, quadrant.Item2];
      var oldBucket = GetBucket(entity.BucketId);
      entity.BucketId = newBucket.BucketId;

      lock(this){
        oldBucket.Entitites.Remove(entity.Id);
        newBucket.Entitites.Add(entity.Id, entity);
      }
    }

    /// <summary>
    /// Agents the will update.
    /// </summary>
    /// <param name="agent">Agent.</param>
    public void AgentWillUpdate(Agent agent)
    {
      
    }

    /// <summary>
    /// Agents the did update.
    /// </summary>
    /// <param name="agent">Agent.</param>
    public void AgentDidUpdate(Agent agent)
    {
      Reallocate(agent.Entity);
    }
  }
}
