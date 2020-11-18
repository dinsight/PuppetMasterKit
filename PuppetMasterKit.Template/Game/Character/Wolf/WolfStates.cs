using System;
using PuppetMasterKit.Utility.Attributes;

namespace PuppetMasterKit.Template.Game.Character.Wolf
{
  public enum WolfStates
  {
    [StringValue("run")] run,
    [StringValue("idle")] idle,
    [StringValue("attack")] attack
  }
}
