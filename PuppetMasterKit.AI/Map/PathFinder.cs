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
            public int Code { get; set; }
            public int Row { get; set; }
            public int Col { get; set; }
            public float G { get; set; }
            public float F { get; set; }
            public float H { get; set; }
            public Node Parent { get; set; }

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
        }

        private int neighbourDepthCheck = 2;
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

            while (open.Count > 0) {
                var node = open.MinBy(x => x.F);
                if (node.Equals(end)) {
                    end = node;
                    break;//found destination
                }
                var neighbours = walkable
                    .Where(IsNeighbor(node))
                    //we don't want the path to cross over  other rooms' exits
                    .Where(x => x.Code != MapCodes.EXIT ||
                               (x.Code == MapCodes.EXIT && (x.Equals(start) || x.Equals(end))));

                foreach (var neighbor in neighbours) {
                    if (closed.Contains(neighbor)) {
                        continue;
                    }
                    neighbor.G = neighbor.G + Point.Distance(node.Row, node.Col, neighbor.Row, neighbor.Col); ;
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
        /// <param name="map"></param>
        /// <param name="node"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        private float Heuristics(int[,] map, Node current, Node node, Node dest)
        {
            var cost = Point.Distance(node.Row, node.Col, dest.Row, dest.Col);
            var turns = CountTurns(current) + (IsTurn(current, node) ? 1 : 0);
            return cost * (turns+1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        private bool IsTurn(Node from, Node next) {
            return next.Row - from.Row != 0 && next.Col - from.Col != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromNode"></param>
        /// <returns></returns>
        private int CountTurns(Node fromNode)
        {
            var count = 0;
            var current = fromNode;
            while (current != null) {
                var prev = current.Parent;
                if (prev != null) {
                    if (IsTurn(current, prev))
                        count++;
                }
                current = prev;
            }
            return count;
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
                result.Add(Tuple.Create(node.Row, node.Col));
                node = node.Parent;
            }
            result.Reverse();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static Func<Node, bool> IsNeighbor(Node node)
        {
            return x =>
                (x.Row == node.Row - 1 && x.Col == node.Col) ||
                (x.Row == node.Row + 1 && x.Col == node.Col) ||
                (x.Row == node.Row && x.Col == node.Col - 1) ||
                (x.Row == node.Row && x.Col == node.Col + 1);
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
                    //if (map[r, c] == MapCodes.EXIT || map[r, c] == MapBuilder.Blank || map[r, c] == MapCodes.PATH ) {
                        yield return new Node() { Row = r, Col = c, Code = map[r, c] };
                    //}
                }
            }
        }
    }
}
