﻿namespace PuppetMasterKit.Utility.Noise
{
  public interface INoiseGenerator
  {
    float Noise(float x, float y);

    int XDim { get; }

    int YDim { get; }
  }
}