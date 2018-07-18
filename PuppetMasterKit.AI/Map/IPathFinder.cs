using System;
using System.Collections.Generic;
using Pair = System.Tuple<int, int>;

namespace PuppetMasterKit.AI.Map
{
  public interface IPathFinder
  {
    List<Pair> Find(int[,] map, int rowFrom, int colFrom, int rowTo, int colTo);
  }
}
