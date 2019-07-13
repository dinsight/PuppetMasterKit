using System;
using PuppetMasterKit.Utility.Attributes;

namespace PuppetMasterKit.Template.Game.Character.Rabbit
{
  public enum RabbitStates
  {
    [StringValue("idle")] idle,
    [StringValue("run")] run,
    [StringValue("walk")] walk,
  }
}
