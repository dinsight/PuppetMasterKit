using System;
namespace PuppetMasterKit.AI
{
  public class CollisionState
  {
    public CollisionStatus Status { get; set; }

    public double ElapsedTime { get; set; }

    public double StopWatchValue { get; set; }

    public void ResetStopWatch()
    {
      StopWatchValue = 0;
    }
  }
}
