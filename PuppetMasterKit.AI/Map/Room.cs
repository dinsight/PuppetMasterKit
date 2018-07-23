using System;
using System.Collections.Generic;
using Pair = System.Tuple<int, int>;

namespace PuppetMasterKit.AI.Map
{
  public class Room
    {
        private Module module;
        private int row;
        private int col;
        private List<Pair> tiles;

        public int Id { get; set; }
        public int PathCount { get; set; }
        public int Row { get => row; set => row = value; }
        public int Col { get => col; set => col = value; }
        public Module Module { get => module; }

        public Room(Module module, int[,] map, int row, int col) {
            this.module = module;
            this.row = row;
            this.col = col;
            this.tiles = module.GetTilesFromPositions(map, row, col).ToList();
        }

        public bool IsInside(int row, int col) 
        {
            return tiles.Any(x => x.Item1 == row && x.Item2 == col);
        }
    }
}
