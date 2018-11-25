using System;
namespace PuppetMasterKit.Utility
{
  public static class Measure
  {
    public static void Timed(string description ,Action action) {
      var start = DateTime.Now;
      action();
      var end = DateTime.Now;
      System.Diagnostics.Debug.WriteLine($"{description} took {(end-start).TotalMilliseconds} ms");
    }
  }
}
