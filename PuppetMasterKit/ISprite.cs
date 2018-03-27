using System;
using System.Collections.Generic;
using PuppetMasterKit.Geometry;

namespace PuppetMasterKit
{
    public interface ISprite
    {
        Dictionary<String,Object> UserData{ get; set;}

        float Alpha { get; set; }

        Point Position { get; set; }

        Point AnchorPoint { get; set; }

        void RemoveFromParent();
    }
}
