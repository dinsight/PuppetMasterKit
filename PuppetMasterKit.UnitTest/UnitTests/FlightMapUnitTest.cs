
using System;
using System.Linq;
using NUnit.Framework;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Components;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.UnitTest
{
  [TestFixture]
  public class FlightMapUnitTest
  {
    [Test]
    public void FlightMap_Quadrant()
    {
      var flightMap = new FlightMap(100,100);
      Assert.AreEqual(flightMap.GetPartition(new Point(5,5)), new Tuple<int,int>(0,0));
      Assert.AreEqual(flightMap.GetPartition(new Point(10, 10)), new Tuple<int, int>(1, 1));
      Assert.AreEqual(flightMap.GetPartition(new Point(20, 20)), new Tuple<int, int>(2, 2));
      Assert.AreEqual(flightMap.GetPartition(new Point(20.1f, 20.1f)), new Tuple<int, int>(2, 2));
      Assert.AreEqual(flightMap.GetPartition(new Point(100f, 100f)), new Tuple<int, int>(9, 9));
    }

    /// <summary>
    /// 
    /// </summary>
    [Test]
    public void FlightMap_PosUpdate()
    {
      var flightMap = new FlightMap(10, 10);
      var cs = new ComponentSystem();
      var agent = new Agent();
      var entity = EntityBuilder.Build().With(cs, agent).GetEntity();

      agent.Position = Point.Zero;
      flightMap.Add(entity);
      flightMap.AgentDidUpdate(agent);

      var bucket = flightMap.GetBucket(entity.BucketId);
      Assert.True(bucket.BucketId.X == 0 && bucket.BucketId.Y ==0);

      agent.Position = new Point(2.3f, 2.5f);
      flightMap.AgentDidUpdate(agent);
      bucket = flightMap.GetBucket(entity.BucketId);
      Assert.True(bucket.BucketId.X == 2 && bucket.BucketId.Y == 2);

      agent.Position = new Point(10f, 10f);
      flightMap.AgentDidUpdate(agent);
      bucket = flightMap.GetBucket(entity.BucketId);
      Assert.True(bucket.BucketId.X == 9 && bucket.BucketId.Y == 9);
    }

    /// <summary>
    /// 
    /// </summary>
    [Test]
    public void FlightMap_TestAdjacent()
    {
      var flightMap = new FlightMap(10, 10);
      var cs = new ComponentSystem();
      var agent = new Agent();
      var entity = EntityBuilder.Build().With(cs, agent).GetEntity();

      agent.Position = new Point(1.5f, 1.5f);
      flightMap.Add(entity);

      //list of neighbors
      {
        flightMap.Add(EntityBuilder.Build().With(cs, new Agent() { Position = new Point(0.5f,0.5f) }).GetEntity());
        flightMap.Add(EntityBuilder.Build().With(cs, new Agent() { Position = new Point(1.5f, 0.5f) }).GetEntity());
        flightMap.Add(EntityBuilder.Build().With(cs, new Agent() { Position = new Point(2f, 0.5f) }).GetEntity());

        flightMap.Add(EntityBuilder.Build().With(cs, new Agent() { Position = new Point(0.1f, 1.7f) }).GetEntity());
        flightMap.Add(EntityBuilder.Build().With(cs, new Agent() { Position = new Point(1.1f, 1.7f) }).GetEntity());
        flightMap.Add(EntityBuilder.Build().With(cs, new Agent() { Position = new Point(2.1f, 1.7f) }).GetEntity());

        flightMap.Add(EntityBuilder.Build().With(cs, new Agent() { Position = new Point(0.1f, 2.7f) }).GetEntity());
        flightMap.Add(EntityBuilder.Build().With(cs, new Agent() { Position = new Point(1.1f, 2.7f) }).GetEntity());
        flightMap.Add(EntityBuilder.Build().With(cs, new Agent() { Position = new Point(2.1f, 2.7f) }).GetEntity());

        flightMap.Add(EntityBuilder.Build().With(cs, new Agent() { Position = new Point(0.1f, 3.7f) }).GetEntity());
        flightMap.Add(EntityBuilder.Build().With(cs, new Agent() { Position = new Point(1.1f, 3.7f) }).GetEntity());
        flightMap.Add(EntityBuilder.Build().With(cs, new Agent() { Position = new Point(2.1f, 3.7f) }).GetEntity());
      }

      var adjacent = flightMap.GetAdjacentEntities(entity, x=>true).ToList();
      Assert.True(adjacent.Count == 9);
    }
  }
}
