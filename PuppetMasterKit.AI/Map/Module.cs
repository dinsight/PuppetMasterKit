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
    //public static int PATH = 5;
    //public static int PADDING = 4;
    //public static int CENTER = 3;
    //public static int EXIT = 2;
    //public static int X = 1;

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

    private Pair GetPatternCenter() => FilterPattern(v => v == MapCodes.CENTER).FirstOrDefault();

    private List<Pair> GetPatternExits() => FilterPattern(v => v == MapCodes.EXIT).ToList();

    private void SetIfNotEmpty(int val, int row, int col) => Set(val, row, col, true);

    public void Set(int val, int row, int col) => Set(val, row, col, false);

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

    private void Apply(Action<int, int, int> action)
    {
      for (int r = 0; r < Rows; r++) {
        for (int c = 0; c < Cols; c++) {
          action(r, c, pattern[r, c]);
        }
      }
    }

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

    private int this[int row, int col] {
      get {
        return pattern[row, col];
      }
    }

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

    public IEnumerable<Pair> GetExitsForPosition(int[,] map, int row, int col)
    {
      var patternExits = GetPatternExits();
      foreach (var exit in patternExits) {
        var dr = exit.Item1 - patternCenter.Item1;
        var dc = exit.Item2 - patternCenter.Item2;
        yield return Tuple.Create(row + dr, col + dc);
      }
    }
  }
}
