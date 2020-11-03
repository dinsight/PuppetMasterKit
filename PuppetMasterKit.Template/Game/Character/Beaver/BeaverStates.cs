using System;
using PuppetMasterKit.Utility.Attributes;

namespace PuppetMasterKit.Template.Game.Character.Rabbit
{
  public enum BeaverStates
  {
    [StringValue("idle")] idle,
    [StringValue("build")] build,
    [StringValue("walk")] walk,
    [StringValue("chop")] chop,
  }
}
