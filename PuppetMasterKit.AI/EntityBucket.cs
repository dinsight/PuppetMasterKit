using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using PuppetMasterKit.Graphics.Geometry;

namespace PuppetMasterKit.AI
{
  public class EntityBucket
  {
    public EntityBucketId BucketId { get; set; }

    public Dictionary<String, Entity> Entitites = new Dictionary<string, Entity>();
  }
}
