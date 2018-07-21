using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuppetMasterKit.Utility;
using Pair = System.Tuple<int, int>;

namespace PuppetMasterKit.AI.Map
{
  public class Module
  {
    private int[,] pattern;
    private Pair patternCenter;

    public int Rows { get => pattern.GetLength(0); }

    public int Cols { get => pattern.GetLength(1); }

    public Module(int[,] pattern)
    {
      this.pattern = pattern;
      patternCenter = GetPatternCenter();
      if (patternCenter == null) {
        throw new ArgumentException("The pattern is missing a center point");
      }
    }
    /// <summary>
    /// Gets the pattern center.
    /// </summary>
    /// <returns>The pattern center.</returns>
    private Pair GetPatternCenter() => FilterPattern(v => v == MapCodes.CENTER).FirstOrDefault();

    /// <summary>
    /// Gets the pattern exits.
    /// </summary>
    /// <returns>The pattern exits.</returns>
    private List<Pair> GetPatternExits() => FilterPattern(v => v == MapCodes.EXIT).ToList();

    /// <summary>
    /// Sets if not empty.
    /// </summary>
    /// <param name="val">Value.</param>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    private void SetIfNotEmpty(int val, int row, int col) => Set(val, row, col, true);

    /// <summary>
    /// Set the specified val, row and col.
    /// </summary>
    /// <param name="val">Value.</param>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    public void Set(int val, int row, int col) => Set(val, row, col, false);

    /// <summary>
    /// 
    /// </summary>
    /// <returns>The pattern.</returns>
    /// <param name="filter">Filter.</param>
    private IEnumerable<Pair> FilterPattern(Func<int, bool> filter)
    {
      for (int r = 0; r < Rows; r++) {
        for (int c = 0; c < Cols; c++) {
          if (filter(pattern[r, c])) {
            yield return Tuple.Create(r, c);
          }
        }
      }
    }

    /// <summary>
    /// Apply the specified action.
    /// </summary>
    /// <param name="action">Action.</param>
    private void Apply(Action<int, int, int> action)
    {
      for (int r = 0; r < Rows; r++) {
        for (int c = 0; c < Cols; c++) {
          action(r, c, pattern[r, c]);
        }
      }
    }

    /// <summary>
    /// Set the specified val, row, col and ignoreIfSet.
    /// </summary>
    /// <param name="val">Value.</param>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    /// <param name="ignoreIfSet">If set to <c>true</c> ignore if set.</param>
    private void Set(int val, int row, int col, bool ignoreIfSet)
    {
      if (row >= 0 && row < Rows &&
          col >= 0 && col < Cols) {
        var isSet = pattern[row, col] != 0;
        if (!ignoreIfSet || !isSet) {
          pattern[row, col] = val;
        }
      }
    }

    /// <summary>
    /// Gets the <see cref="T:PuppetMasterKit.AI.Map.Module"/> with the specified row col.
    /// </summary>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    private int this[int row, int col] {
      get {
        return pattern[row, col];
      }
    }

    /// <summary>
    /// Pad the specified padding.
    /// </summary>
    /// <returns>The pad.</returns>
    /// <param name="padding">Padding.</param>
    public Module Pad(int padding)
    {
      var newPattern = new int[Rows + 2 * padding, Cols + 2 * padding];
      Stamp(newPattern, patternCenter.Item1 + padding, patternCenter.Item2 + padding);
      var module = new Module(newPattern);
      while (padding-- > 0) {
        module.Apply((r, c, v) => {
          if (v != 0 && v != MapCodes.PADDING) {
            module.SetIfNotEmpty(MapCodes.PADDING, r - 1, c - 1);
            module.SetIfNotEmpty(MapCodes.PADDING, r - 1, c);
            module.SetIfNotEmpty(MapCodes.PADDING, r - 1, c + 1);
            module.SetIfNotEmpty(MapCodes.PADDING, r, c - 1);
            module.SetIfNotEmpty(MapCodes.PADDING, r, c + 1);
            module.SetIfNotEmpty(MapCodes.PADDING, r + 1, c - 1);
            module.SetIfNotEmpty(MapCodes.PADDING, r + 1, c);
            module.SetIfNotEmpty(MapCodes.PADDING, r + 1, c + 1);
          }
        });

        module.Apply((r, c, v) => {
          if (v == MapCodes.PADDING) {
            module.Set(MapCodes.X, r, c);
          }
        });
      }
      return module;
    }

    /// <summary>
    /// Stamp the specified map, row and col.
    /// </summary>
    /// <param name="map">Map.</param>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    public void Stamp(int[,] map, int row, int col)
    {
      var rows = map.GetLength(0);
      var cols = map.GetLength(1);
      FilterPattern(v => v != 0)
          .ToList()
          .ForEach(x => {
            var dr = x.Item1 - patternCenter.Item1;
            var dc = x.Item2 - patternCenter.Item2;
            if (row + dr >= 0 && row + dr < rows &&
                      col + dc >= 0 && col + dc < cols) {
              map[row + dr, col + dc] = pattern[x.Item1, x.Item2];
            }
          });
    }

    /// <summary>
    /// Cans the fit.
    /// </summary>
    /// <returns><c>true</c>, if fit was caned, <c>false</c> otherwise.</returns>
    /// <param name="map">Map.</param>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    public bool CanFit(int[,] map, int row, int col)
    {
      var rows = map.GetLength(0);
      var cols = map.GetLength(1);
      var overlap = FilterPattern(v => v != 0)
          .ToList()
          .FirstOrDefault(x => {
            var dr = x.Item1 - patternCenter.Item1;
            var dc = x.Item2 - patternCenter.Item2;
            if (row + dr >= 0 && row + dr < rows &&
                      col + dc >= 0 && col + dc < cols) {
              if (map[row + dr, col + dc] != MapBuilder.Blank) {
                return true;
              }
            } else {
              return true;
            }
            return false;
          });
      return overlap == null;
    }

    /// <summary>
    /// Gets the exits for position.
    /// </summary>
    /// <returns>The exits for position.</returns>
    /// <param name="map">Map.</param>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    public IEnumerable<Pair> GetExitsForPosition(int[,] map, int row, int col)
    {
      var patternExits = GetPatternExits();
      foreach (var exit in patternExits) {
        var dr = exit.Item1 - patternCenter.Item1;
        var dc = exit.Item2 - patternCenter.Item2;
        if (map[row + dr, col + dc] == MapCodes.EXIT) {
          yield return Tuple.Create(row + dr, col + dc);
        }
      }
    }
  }
}
