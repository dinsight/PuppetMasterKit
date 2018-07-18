using System;
using System.Linq;
using System.Collections.Generic;
using Pair = System.Tuple<int, int>;
using PuppetMasterKit.Utility;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI.Map
{
  public class PathFinder : IPathFinder
  {
    private class Node
    {
      public int Row { get; set; }
      public int Col { get; set; }
      public float GCost { get; set; }
      public float FCost { get; set; }
      public Node Parent { get; set; }

      public Node()
      {
        FCost = float.MaxValue;
        GCost = float.MaxValue;
      }

      public override bool Equals(object obj)
      {
        var node = obj as Node;
        if (node == null)
          return false;
        return this.Row == node.Row && this.Col == node.Col;
      }

      public override int GetHashCode()
      {
        return base.GetHashCode();
      }
    }

    /// <summary>
    /// Find the specified map, rowFrom, colFrom, rowTo and colTo.
    /// </summary>
    /// <returns>The find.</returns>
    /// <param name="map">Map.</param>
    /// <param name="rowFrom">Row from.</param>
    /// <param name="colFrom">Col from.</param>
    /// <param name="rowTo">Row to.</param>
    /// <param name="colTo">Col to.</param>
    public List<Pair> Find(int[,] map, int rowFrom, int colFrom, int rowTo, int colTo)
    {
      var walkable = GetWalkableTiles(map);
      var start = walkable.FirstOrDefault(x => x.Row == rowFrom && x.Col == colFrom);
      var end = walkable.FirstOrDefault(x => x.Row == rowTo && x.Col == colTo);
      //ensure the start and end tiles are walkable
      if (start == null || end == null) {
        return new List<Pair>();
      }
      var open = new List<Node>();
      var closed = new List<Node>();
      open.Add(start);
      start.GCost = 0;

      while (open.Count > 0) {
        var node = open.MinBy(x => x.FCost);
        if (node.Equals(end)) {
          end = node;
          break;//found destination
        }
        var neighbours = GetNeighbours(walkable, node);
        foreach (var neighbor in neighbours) {
          if(closed.Contains(neighbor)){
            continue;
          }
          var cost = neighbor.GCost + Point.Distance(node.Row, node.Col, neighbor.Row, neighbor.Col);
          if(!open.Contains(neighbor)){
            open.Add(neighbor);
          } else if(cost >= neighbor.GCost){
            continue;
          }
          neighbor.GCost = cost;
          neighbor.FCost = neighbor.GCost + Point.Distance(neighbor.Row, neighbor.Col, end.Row, end.Col);
          neighbor.Parent = node;
        }
        closed.Add(node);
        open.Remove(node);
      }

      return GetPath(end);
    }

    /// <summary>
    /// Gets the path.
    /// </summary>
    /// <returns>The path.</returns>
    /// <param name="end">End.</param>
    private List<Pair> GetPath(Node end){
      var result = new List<Pair>();
      var node = end;
      while(node!=null){
        result.Add(Tuple.Create(node.Row, node.Col));
        node = node.Parent;
      }
      result.Reverse();
      return result;
    }

    /// <summary>
    /// Gets the successors.
    /// </summary>
    /// <returns>The successors.</returns>
    /// <param name="walkable">Walkable.</param>
    /// <param name="node">Node.</param>
    private IEnumerable<Node> GetNeighbours(IEnumerable<Node> walkable, Node node)
    {
      var result = walkable
        .Where(x =>
               (x.Row == node.Row - 1 && x.Col == node.Col) ||
               (x.Row == node.Row + 1 && x.Col == node.Col) ||
               (x.Row == node.Row && x.Col == node.Col - 1) ||
               (x.Row == node.Row && x.Col == node.Col + 1)
              );

      return result;
    }

    /// <summary>
    /// Gets the walkable tiles.
    /// </summary>
    /// <returns>The walkable tiles.</returns>
    /// <param name="map">Map.</param>
    private IEnumerable<Node> GetWalkableTiles(int[,] map)
    {
      var rows = map.GetLength(0);
      var cols = map.GetLength(1);
      for (int r = 0; r < rows; r++) {
        for (int c = 0; c < cols; c++) {
          if (map[r, c] == MapBuilder.Blank) {
            yield return new Node() { Row = r, Col = c };
          }
        }
      }
    }
  }
}
