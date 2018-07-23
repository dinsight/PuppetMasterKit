using System;
using System.Collections.Generic;
using Pair = System.Tuple<int, int>;

namespace PuppetMasterKit.AI.Map
{
  public class Room
  {
    Module module;
    int row;
    int col;

    public int Id { get; set; }
    public int PathCount { get; set; }
    public int Row { get => row; set => row = value; }
    public int Col { get => col; set => col = value; }
    public Module Module { get => module; }

    public Room(Module module, int row, int col)
    {
      this.module = module;
      this.row = row;
      this.col = col;
    }

    internal IEnumerable<Pair> GetExits(int[,] map)
    {
      return module.GetExitsForPosition(map, Row, Col);
    }
  }
}
