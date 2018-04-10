using System;
namespace PuppetMasterKit.AI
{
  public class EntityBucketId
  {
    public int Key1 { get; private set; }
    public int Key2 { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.EntityBucketId"/> class.
    /// </summary>
    /// <param name="i">The index.</param>
    /// <param name="j">J.</param>
    public EntityBucketId(int i, int j)
    {
      this.Key1 = i;
      this.Key2 = j;
    }

  }
}
