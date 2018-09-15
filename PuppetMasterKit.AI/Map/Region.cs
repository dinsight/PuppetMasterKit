using PuppetMasterKit.AI.Map;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Pair = System.Tuple<int, int>;

namespace PuppetMasterKit.AI
{
  public class Region
  {
    public int RegionFill { get; }

    public IReadOnlyCollection<Pair> Tiles {
      get {
        return new ReadOnlyCollection<Pair>(tiles.Values.ToList());
      }
    }

    public int MinRow { get => minRow; }
    public int MaxRow { get => maxRow; }
    public int MinCol { get => minCol; }
    public int MaxCol { get => maxCol; }

    private Dictionary<int, Pair> tiles = new Dictionary<int, Pair>();
    private int minRow = int.MaxValue;
    private int maxRow = int.MinValue;
    private int minCol = int.MaxValue;
    private int maxCol = int.MinValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Region"/> class.
    /// </summary>
    /// <param name="regionFill">Region fill.</param>
    public Region(int regionFill)
    {
      this.RegionFill = regionFill;
    }

    /// <summary>
    /// https://en.wikipedia.org/wiki/Pairing_function#Cantor_pairing_function
    /// </summary>
    /// <returns></returns>
    /// <param name="">.</param>
    private int Cantor(int a, int b)
    {
      return (a + b) * (a + b + 1) / 2 + b;
    }

    /// <summary>
    /// Adds the tile.
    /// </summary>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    public void AddTile(int row, int col)
    {
      tiles.Add(Cantor(row, col), Tuple.Create(row, col));
      minRow = Math.Min(MinRow, row);
      maxRow = Math.Max(MaxRow, row);
      minCol = Math.Min(MinCol, col);
      maxCol = Math.Max(MaxCol, col);
    }

    /// <summary>
    /// Gets the <see cref="T:PuppetMasterKit.AI.Region"/> with the specified row col.
    /// </summary>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    public Pair this[int row, int col] {
      get {
        Pair value = null;
        tiles.TryGetValue(Cantor(row, col), out value);
        return value;
      }
    }
  }
}
