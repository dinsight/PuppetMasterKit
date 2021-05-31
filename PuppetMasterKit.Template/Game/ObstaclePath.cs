using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.AI;
using PuppetMasterKit.Graphics.Geometry;

using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.Graphics.Sprites;
using LightInject;
using PuppetMasterKit.Terrain.Map;

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static List<Point> FindPath(Point from, Point to) {
      return FindPath(from, to, (r, c, v) => v == 0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="board"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static List<Point> FindPath(Point from, Point to, Func<int, int, int, bool> tileFilter)
    {
      var pathFinder = new PathFinder();
      var flightMap  = Container.GetContainer().GetInstance<FlightMap>() as GameFlightMap;
      var tileSize   = flightMap.MapWidth / flightMap.MapRows;
      var fromRow    = (int)(from.X / tileSize);
      var fromCol    = (int)(from.Y / tileSize);
      var toRow      = (int)(to.X / tileSize);
      var toCol      = (int)(to.Y / tileSize);
      var boardIndexPath = pathFinder.Find(
        flightMap.Board,
        fromRow,fromCol,
        toRow, toCol, tileFilter);

      var path = new List<Point>();
      boardIndexPath.ForEach(p => {
        var intermediate = new Point(
          p.Item1 * tileSize + tileSize / 2,
          p.Item2 * tileSize + tileSize / 2);
        path.Add(intermediate);
      });

      return path;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public static Point FindClosestWalkableTile(Point from) {
      var flightMap = Container.GetContainer().GetInstance<FlightMap>() as GameFlightMap;
      var tileSize = flightMap.MapWidth / flightMap.MapRows;
      var fromRow = (int)(from.X / tileSize);
      var fromCol = (int)(from.Y / tileSize);
      
      var minDist = float.MaxValue;
      var minRow = -1;
      var minCol = -1;
      for (int r = 0; r < flightMap.MapRows; r++) {
        for (int c = 0; c < flightMap.MapCols; c++) {
          if (flightMap.Board[r, c]==0) {
            var dist = Point.Distance(fromRow,fromCol,r,c);
            if (dist < minDist) {
              minDist = dist;
              minRow = r;
              minCol = c;
            }
          }
        }
      }

      if (minDist != float.MaxValue) {
        return
          new Point(
          minRow * tileSize + tileSize / 2,
          minCol * tileSize + tileSize / 2);
      }
      return null;
    }
  }
}
