using System;
namespace PuppetMasterKit.Terrain.Map
{
  public enum TileType
  {
    Unknown,
    Plain,
    BottomLeftCorner,
    BottomRightCorner,
    TopLeftCorner,
    TopRightCorner,
    TopSide,
    BottomSide,
    LeftSide,
    RightSide,
    TopLeftJoint,
    TopRightJoint,
    BottomLeftJoint,
    BottomRightJoint,
    CulDeSacTop,
    CulDeSacBottom,
    CulDeSacLeft,
    CulDeSacRight,
  }
}
