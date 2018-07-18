using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.AI;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.Template.Game
{
  public class ObstaclePath
  {
    private class ObstacleInfo
    {
      public Polygon Obstacle { get; set; }
      public int FirstSegmentIndex {get; set;}
      public int LastSegmentIndex { get; set; }
      public float DistanceToPolygon { get; set; }
    }
    /// <summary>
    /// Gets the path trough obstacles.
    /// </summary>
    /// <returns>The path trough obstacles.</returns>
    /// <param name="obstacles">Obstacles.</param>
    /// <param name="from">From.</param>
    /// <param name="to">To.</param>
    public static List<Point> GetPathTroughObstacles(List<PolygonalObstacle> obstacles, Point from, Point to)
    {
      var path = new List<Point>();
      var obstaclesInPath = new List<ObstacleInfo>();
      var segment = new Segment(from, to);
      path.Add(from);
      foreach (var item in obstacles) {
        var obstacleInfo = Intersect(item.Polygon, segment);
        if(obstacleInfo!=null){
          obstaclesInPath.Add(obstacleInfo);
        }
      }
      obstaclesInPath.Sort((x, y) => (int)(x.DistanceToPolygon - y.DistanceToPolygon));
      obstaclesInPath.ForEach(x => {
        var cwr = GetAvoidanceRoute(x, true);
        var ccwr = GetAvoidanceRoute(x, false);
        var cwp = GetPathDistance(cwr);
        var ccwp = GetPathDistance(ccwr);
        cwp += Point.Distance(from, cwr.FirstOrDefault()) + 
               Point.Distance(cwr.LastOrDefault(), to) ;
        ccwp += Point.Distance(from, ccwr.FirstOrDefault()) +
               Point.Distance(ccwr.LastOrDefault(), to);
        if(cwp < ccwp){
          path.AddRange(cwr);
        } else {
          path.AddRange(ccwr);
        }
      });
      path.Add(to);
      return path;
    }

    /// <summary>
    /// Gets the avoidance route.
    /// </summary>
    /// <returns>The avoidance route.</returns>
    /// <param name="obstacleInfo">Obstacle info.</param>
    private static List<Point> GetAvoidanceRoute(ObstacleInfo obstacleInfo, bool clockwise= true)
    {
      var N = obstacleInfo.Obstacle.Count;
      var start = obstacleInfo.FirstSegmentIndex;
      var end = obstacleInfo.LastSegmentIndex;
      var route = new List<Point>();

      if(clockwise){
        var index = start + 1;
        do {
          route.Add(obstacleInfo.Obstacle[index % N]);
          index++;
        } while (index % N != (end + 1) % N);
      } else {
        var index = start;
        while (index % N != end % N) {
          route.Add(obstacleInfo.Obstacle[index % N]);
          index--;
          if (index < 0)
            index = N - index;
        }  
      }
      return route;
    }

    /// <summary>
    /// Calculates the path's distance
    /// </summary>
    /// <returns>The path distance.</returns>
    /// <param name="path">Path.</param>
    private static float GetPathDistance(List<Point> path)
    {
      if (path.Count == 0)
        return 0;
      var prev = path.First();
      var distance = 0f;
      path.ForEach(x =>{
        distance += Point.Distance(prev, x);
        prev = x;
      });
      return distance;
    }

    /// <summary>
    /// Intersect the specified polygon and segment.
    /// </summary>
    /// <returns>The intersect.</returns>
    /// <param name="polygon">Polygon.</param>
    /// <param name="segment">Segment.</param>
    private static ObstacleInfo Intersect(Polygon polygon, Segment segment)
    {
      float minDistance = float.MaxValue;
      float maxDistance = float.MinValue;
      int closestEdgeIndex = -1;
      int farthestEdgeIndex = -1;

      for (int index = 0; index < polygon.Count; index++) {
        var edge = polygon.GetEdge(index);
        var intp = edge.Intersect(segment);
        if (intp.Count == 0)
          continue;
        
        var distToEdge = intp.Select(x=>Point.Distance(segment.Start, x)).Min();
        if(distToEdge>maxDistance){
          maxDistance = distToEdge;
          farthestEdgeIndex = index;
        }
        if(distToEdge<minDistance){
          minDistance = distToEdge;
          closestEdgeIndex = index;
        }
      }

      if(closestEdgeIndex==-1 && farthestEdgeIndex == -1){
        return null;
      }

      return new ObstacleInfo() {
        Obstacle = polygon,
        DistanceToPolygon = minDistance,
        FirstSegmentIndex = closestEdgeIndex,
        LastSegmentIndex = farthestEdgeIndex
      };
    }
  }
}
