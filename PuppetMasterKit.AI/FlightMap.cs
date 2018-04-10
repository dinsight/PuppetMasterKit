using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.AI.Components;
using System.Threading;

namespace PuppetMasterKit.AI
{
  public class FlightMap : IAgentDelegate
  {

    private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

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
    /// Releases unmanaged resources and performs other cleanup operations before the
    /// <see cref="T:PuppetMasterKit.AI.FlightMap"/> is reclaimed by garbage collection.
    /// </summary>
    ~FlightMap(){
     if (rwLock != null) {
        rwLock.Dispose();
      }
    }

    /// <summary>
    /// Wraps the read lock.
    /// </summary>
    /// <returns>The read lock.</returns>
    /// <param name="func">Func.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    private T ReadLock<T>(Func<T> func)
    {
      rwLock.EnterReadLock();
      try{
        return func();
      } finally {
        rwLock.ExitReadLock();
      }
    }

    /// <summary>
    /// Wraps the write lock.
    /// </summary>
    /// <param name="func">Func.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    private void WriteLock(Action func)
    {
      rwLock.EnterWriteLock();
      try {
        func();
      } finally {
        rwLock.ExitWriteLock();
      }
    }

    /// <summary>
    /// Gets the entity by identifier.
    /// </summary>
    /// <returns>The by identifier.</returns>
    /// <param name="id">Identifier.</param>
    public Entity GetEntityById(String id)
    {
      return ReadLock(()=>{
        Entity temp;
        Entities.TryGetValue(id, out temp);
        return temp;
      });
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

      WriteLock(() => {
        Entities.Add(entity.Id, entity);
        bucket.Entitites.Add(entity.Id, entity);
      });
    }

    /// <summary>
    /// Remove the specified entity.
    /// </summary>
    /// <returns>The remove.</returns>
    /// <param name="entity">Entity.</param>
    public void Remove(Entity entity)
    {
      WriteLock(() => { 
        if (entity.BucketId != null) {
          var bucket = GetBucket(entity.BucketId);
          bucket.Entitites.Remove(entity.Id);
          Entities.Remove(entity.Id);
        } else {
          Entities.Remove(entity.Id);
        } 
      });
    }

    /// <summary>
    /// Gets the entities.
    /// </summary>
    /// <returns>The entities.</returns>
    /// <param name="predicate">Predicate.</param>
    public IEnumerable<Entity> GetEntities(Predicate<Entity> predicate)
    {
      return ReadLock(()=>{
        return new List<Entity>(Entities.Values.Where(x => predicate(x)));
      });
    }

    /// <summary>
    /// Gets the adjacent entities.
    /// </summary>
    /// <returns>The adjacent entities.</returns>
    /// <param name="entity">Entity.</param>
    /// <param name="predicate">Predicate.</param>
    /// <param name="depth">Depth.</param>
    public IEnumerable<Entity> GetAdjacentEntities(Entity entity, 
                                                   Predicate<Entity> predicate, 
                                                   int depth=1)
    {
      var startX = Math.Max(0, entity.BucketId.X - depth);
      var endX   = Math.Min(partitionsCountX-1 , entity.BucketId.X + depth);
      var startY = Math.Max(0, entity.BucketId.Y - depth);
      var endY   = Math.Min(partitionsCountY-1, entity.BucketId.Y + depth);

      return ReadLock(()=>{
        var adjacent = new List<Entity>();
        for (int i = startX; i <= endX; i++) {
          for (int j = startY; j <= endY; j++) {
            adjacent.AddRange(Buckets[i, j].Entitites.Values);
          }
        }
        return adjacent.Where(x=>predicate(x) && !Object.ReferenceEquals(x, entity));
      });
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
      if (id.X <0 || id.Y < 0 
          || id.X >= partitionsCountX 
          || id.Y >= partitionsCountY ) {
        return null;
      }
      var bucket = this.Buckets[id.X, id.Y];

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
      if (entity.BucketId.X == quadrant.Item1 && 
          entity.BucketId.Y == quadrant.Item2)
        return;

      var newBucket = Buckets[quadrant.Item1, quadrant.Item2];
      var oldBucket = GetBucket(entity.BucketId);
      entity.BucketId = newBucket.BucketId;

      WriteLock(()=>{
        oldBucket.Entitites.Remove(entity.Id);
        newBucket.Entitites.Add(entity.Id, entity);  
      });
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
