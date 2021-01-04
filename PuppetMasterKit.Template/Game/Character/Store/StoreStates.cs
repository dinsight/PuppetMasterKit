using System;
using PuppetMasterKit.Utility.Attributes;

namespace PuppetMasterKit.Template.Game.Character.Rabbit
{
  public enum StoreStates
  {
    [StringValue("building")] building,
    [StringValue("full")] full,
    [StringValue("depleted")] depleted
  }
}
