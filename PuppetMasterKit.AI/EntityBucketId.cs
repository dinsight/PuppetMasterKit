using System;
namespace PuppetMasterKit.AI
{
  public class EntityBucketId
  {
    public int X { get; private set; }
    public int Y { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.EntityBucketId"/> class.
    /// </summary>
    /// <param name="i">The index.</param>
    /// <param name="j">J.</param>
    public EntityBucketId(int i, int j)
    {
      this.X = i;
      this.Y = j;
    }

  }
}
