using System;
using System.Collections.Generic;
using PuppetMasterKit.Utility.Attributes;

namespace PuppetMasterKit.Template.Game.Level
{
  public class TerrainDefinition
  {
    public enum TerrainType {
      [StringValue("Water_Background")]
      WATER = 0,
      [StringValue("Wang_Isles")]
      ISLES = 1,
      [StringValue("Wang_Swamp")]
      SWAMP = 2,
      [StringValue("Upland1")]
      UPLAND = 3,
      [StringValue("Wang_Dirt")]
      DIRT = 4,
      //other terrain features
      OBSTACLE = 50,
      DECK = 51,
    }

    public static Dictionary<int,String> GetMapping()
    {
      return new Dictionary<int, string> {
        { (int)TerrainType.ISLES, TerrainType.ISLES.GetStringValue()},
        { (int)TerrainType.UPLAND, TerrainType.UPLAND.GetStringValue()},
        { (int)TerrainType.SWAMP, TerrainType.SWAMP.GetStringValue()},
        { (int)TerrainType.DIRT, TerrainType.DIRT.GetStringValue()},
        { (int)TerrainType.WATER, TerrainType.WATER.GetStringValue() }
      };
    }
  }
}
