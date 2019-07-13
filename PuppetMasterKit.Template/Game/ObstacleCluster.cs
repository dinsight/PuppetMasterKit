using System;
using System.Linq;
using PuppetMasterKit.Utility.Extensions;
using System.Collections.Generic;
using PuppetMasterKit.AI;
using ClusterMap = System.Collections.Generic.Dictionary<System.Tuple<int, int>, PuppetMasterKit.AI.PolygonalObstacle>;
using ClusterItem = System.Collections.Generic.KeyValuePair<System.Tuple<int, int>, PuppetMasterKit.AI.PolygonalObstacle>;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.Template.Game
{
  public static class ObstacleCluster
  {
    /// <summary>
    /// Clusters the obstacles.
    /// </summary>
    /// <returns>The obstacles.</returns>
    /// <param name="obstacleMap">Obstacle map.</param>
    public static PolygonalObstacle[] CreateClusters(ClusterMap obstacleMap)
    {
      var result = new List<PolygonalObstacle>();
      if (obstacleMap.Values.Count == 0) {
        return result.ToArray();
      }
      var visited = new HashSet<Tuple<int, int>>();
      do {
        var obstacle = obstacleMap.First(x=>!visited.Contains(x.Key));
        var cluster = ClusterFrom(obstacle.Key, obstacleMap, visited);
        var merged = MergePolygons(cluster.ToList());
        TrimPolygon(merged.Polygon);
        result.Add(merged);

      } while (visited.Count != obstacleMap.Count);

      return result.ToArray();
    }

    /// <summary>
    /// Trims the polygon.
    /// </summary>
    /// <param name="polygon">Polygon.</param>
    private static void TrimPolygon(Polygon polygon)
    {
      var toRemove = new List<Point>();
      for (int index = 0; index < polygon.Count; index++) {
        var prev = polygon[index>0?index-1:polygon.Count-1];
        var current = polygon[index];
        var next = polygon[index + 1 == polygon.Count ? 0 : index+1];
        //if(polygon.IsPointInside(current)){
        if(Segment.AreColinear(prev, next, current)){
          toRemove.Add(current);
        }
      }
      toRemove.ForEach(x => polygon.Points.Remove(x));
    }

    /// <summary>
    /// Merges the polygons.
    /// </summary>
    /// <returns>The polygons.</returns>
    /// <param name="cluster">Cluster.</param>
    private static PolygonalObstacle MergePolygons(List<PolygonalObstacle> cluster)
    {
      var obstacle = cluster.FirstOrDefault();
      if (obstacle == null)
        return null;

      //make a copy of the polygon
      var result = obstacle.Polygon.Clone();
      cluster.Skip(1).ForEach(x => MergeAdjacent(result, x.Polygon));
      //return the obstacle
      return new PolygonalObstacle(result.Points.ToArray());
    }

    /// <summary>
    /// Merges the adjacent.
    /// </summary>
    /// <returns>The adjacent.</returns>
    /// <param name="destination">Destination.</param>
    /// <param name="fromSource">From source.</param>
    private static Polygon MergeAdjacent(Polygon destination, Polygon fromSource)
    {
      for (int i = 0; i < destination.Count; i++) {
          var destEdge = destination.GetEdge(i);
          for (int j = 0; j < fromSource.Count; j++) {
          var sourceEdge = fromSource.GetEdge(j);
          if(destEdge.Start == sourceEdge.End && destEdge.End == sourceEdge.Start){
            var sourcePoints = GetPointsBetween(fromSource, j+1, j);
            destination.Insert(sourcePoints,i+1);
            return destination;
          }
        }
      }
      return destination;
    }

    /// <summary>
    /// Gets the points between polygon, startIndex and endIndex.
    /// </summary>
    /// <returns>The <see cref="T:System.Collections.Generic.IEnumerable{PuppetMasterKit.Graphics.Geometry.Point}"/>.</returns>
    /// <param name="polygon">Polygon.</param>
    /// <param name="startIndex">Start index.</param>
    /// <param name="endIndex">End index.</param>
    public static IEnumerable<Point> GetPointsBetween(Polygon polygon, int startIndex, int endIndex)
    {
      var points = new List<Point>();
      var N = polygon.Count;
      var start = (startIndex+1) % N;
      var end = endIndex < start ? endIndex + N : endIndex;
      for (int index = start; index < end; index++) {
        points.Add(polygon[index%N]);
      }

      return points;
    }

    /// <summary>
    /// Cluster the specified obstacle and obstacleMap.
    /// </summary>
    /// <returns>The cluster.</returns>
    /// <param name="obstacle">Obstacle.</param>
    /// <param name="obstacleMap">Obstacle map.</param>
    private static IEnumerable<PolygonalObstacle> ClusterFrom(Tuple<int, int> obstacle, 
                                                 ClusterMap obstacleMap, 
                                                 HashSet<Tuple<int, int>> visited)
    {
      var row = obstacle.Item1;
      var col = obstacle.Item2;
      visited.Add(obstacle);

      var neighbours = new List<Tuple<int,int>> { 
        ValueTuple.Create(row + 1, col).ToTuple(),
        ValueTuple.Create(row - 1, col).ToTuple(),
        ValueTuple.Create(row, col + 1).ToTuple(),
        ValueTuple.Create(row, col - 1).ToTuple()
      };

      yield return obstacleMap[obstacle];

      foreach (var item in neighbours) {
        if (obstacleMap.ContainsKey(item) && !visited.Contains(item)) {
          var ret = ClusterFrom(item, obstacleMap, visited);
          foreach (var poly in ret) {
            yield return poly;
          }
        }
      }
    }
  }
}
