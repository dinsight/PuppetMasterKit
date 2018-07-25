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
    private static int idealMaxPathTurns = 3;
    private Node[,] walkable;
    /// <summary>
    /// Node.
    /// </summary>
    internal class Node
    {
      private Node parent;
      public int Code { get; set; }
      public int Row { get; set; }
      public int Col { get; set; }
      public float G { get; set; }
      public float F { get; set; }
      public float H { get; set; }
      public int CountTurns { get; set; }
      public Node Parent {
        get => parent;
        set {
          parent = value;
          if (parent != null) {
            CountTurns = parent.CountTurns;
            if (parent.IsDirectionChange(this)) {
              CountTurns++;
            }
          }
        }
      }

      public Node()
      {
        F = 0;
        G = 0;
        H = 0;
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

      public bool IsDirectionChange(Node nextNode)
      {
        if (Parent != null) {
          if (this.Row - Parent.Row != nextNode.Row - this.Row ||
              this.Col - Parent.Col != nextNode.Col - this.Col) {
            return true;
          }
        }
        return false;
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
      if (walkable == null) {
        walkable = GetWalkableTiles(map);
      } else {
        ResetNodes(walkable);
      }

      var start = walkable[rowFrom, colFrom];
      var end = walkable[rowTo, colTo];
      //ensure the start and end tiles are walkable
      if (start == null || end == null) {
        return new List<Pair>();
      }
      var open = new List<Node>();
      var closed = new List<Node>();
      open.Add(start);

      while (open.Count > 0) {
        var node = open.MinBy(x => x.F);
        if (node.Equals(end)) {
          end = node;
          break;//found destination
        }
        var neighbours = GetNeighbours(walkable, node);
        foreach (var neighbor in neighbours) {
          if (closed.Contains(neighbor)) {
            continue;
          }
          neighbor.G = node.G +
            Point.Distance(node.Row,
                           node.Col,
                           neighbor.Row,
                           neighbor.Col);

          neighbor.H = Heuristics(map, node, neighbor, end);
          neighbor.F = neighbor.G + neighbor.H;

          if (!open.Contains(neighbor)) {
            open.Add(neighbor);
          } else if (neighbor.G >= node.G) {
            continue;
          }
          neighbor.Parent = node;
        }
        closed.Add(node);
        open.Remove(node);
      }
      return GetPath(end);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="walkable"></param>
    private void ResetNodes(Node[,] walkable)
    {
      for (int i = 0; i < walkable.GetLength(0); i++) {
        for (int j = 0; j < walkable.GetLength(1); j++) {
          var x = walkable[i, j];
          x.CountTurns = 0;
          x.Parent = null;
          x.F = x.G = x.H = 0;
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="node"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    private float Heuristics(int[,] map, Node current, Node node, Node dest)
    {
      var cost = Point.Distance(node.Row, node.Col, dest.Row, dest.Col);
      var turns = current.CountTurns + (current.IsDirectionChange(node) ? 1 : 0);
      return cost * (turns > idealMaxPathTurns ? turns : 1);
    }

    /// <summary>
    /// Gets the path.
    /// </summary>
    /// <returns>The path.</returns>
    /// <param name="end">End.</param>
    private List<Pair> GetPath(Node end)
    {
      var result = new List<Pair>();
      var node = end;
      while (node != null) {
        result.Insert(0, Tuple.Create(node.Row, node.Col));
        node = node.Parent;
      }
      return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private IEnumerable<Node> GetNeighbours(Node[,] nodes, Node node)
    {
      var rows = nodes.GetLength(0);
      var cols = nodes.GetLength(1);
      for (int i = node.Row - 1; i <= node.Row + 1; i++) {
        for (int j = node.Col - 1; j <= node.Col + 1; j++) {
          if (i == node.Row - 1 && j == node.Col - 1 ||
              i == node.Row - 1 && j == node.Col + 1 ||
              i == node.Row + 1 && j == node.Col + 1 ||
              i == node.Row + 1 && j == node.Col - 1)
            continue;

          if (i >= 0 && i < rows && j >= 0 && j < cols && !(node.Row == i && node.Code == j)) {
            yield return nodes[i, j];
          }
        }
      }
    }

    /// <summary>
    /// Gets the walkable tiles.
    /// </summary>
    /// <returns>The walkable tiles.</returns>
    /// <param name="map">Map.</param>
    private Node[,] GetWalkableTiles(int[,] map)
    {
      var nodes = new Node[map.GetLength(0), map.GetLength(1)];
      var rows = map.GetLength(0);
      var cols = map.GetLength(1);
      for (int r = 0; r < rows; r++) {
        for (int c = 0; c < cols; c++) {
          nodes[r, c] = new Node() { Row = r, Col = c, Code = map[r, c] };
        }
      }
      return nodes;
    }
  }
}
