
using System;
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
      Assert.True(bucket.BucketId.Key1 == 0 && bucket.BucketId.Key2 ==0);

      agent.Position = new Point(2.3f, 2.5f);
      flightMap.AgentDidUpdate(agent);
      bucket = flightMap.GetBucket(entity.BucketId);
      Assert.True(bucket.BucketId.Key1 == 2 && bucket.BucketId.Key2 == 2);
    }
  }
}
