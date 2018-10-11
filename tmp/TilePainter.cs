using PuppetMasterKit.Graphics.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrainGen.lib;

namespace TerrainGen
{
  class TilePainter
  {
    int tileSize;

    public TilePainter(int tileSize) {
      this.tileSize = tileSize;
    }
    
    public void PaintLeftSide(String path){ 
      PaintSide(path, (x,y) => Point.Distance(0, 0, x, 0));
    }
    public void PaintRightSide(String path){ 
      PaintSide(path, (x,y) => Point.Distance(tileSize, 0, x, 0));
    }
    public void PaintTopSide(String path){ 
      PaintSide(path, (x,y) => Point.Distance(0, 0, 0, y));
    }
    public void PaintBottomSide(String path){ 
      PaintSide(path, (x,y) => Point.Distance(0, tileSize, 0, y));
    }
    public void PaintTopLeftJoint(String path){ 
      PaintJoint(path, (x,y) => Point.Distance(0, 0, x, y));
    }
    public void PaintTopRightJoint(String path){ 
      PaintJoint(path, (x,y) => Point.Distance(tileSize, 0, x, y));
    }
    public void PaintBottomLeftJoint(String path){ 
      PaintJoint(path, (x,y) => Point.Distance(0, tileSize, x, y));
    }
    public void PaintBottomRightJoint(String path){ 
      PaintJoint(path, (x,y) => Point.Distance(tileSize, tileSize, x, y));
    }

    public void PaintJoint(String path, Func<float, float, float> distFn)
    {
      var r = tileSize/2;
      var d = tileSize*1.414212f;
      using(var output = new PngImageOutput(tileSize , tileSize)){
        for (int x = 0; x < tileSize; x++) {
          for (int y = 0; y < tileSize; y++) {
            var dist = distFn(x,y);
            var c = 0xff*(dist-r)/(d-r);
            var alpha = dist > r ? c : 0x0;
            output.SetPixel(x,y,0xff, 0xff, 0xff, (int)alpha);
          }
        }
        output.Save(path); 
      }
    }

    public void PaintSide(String path, Func<float, float, float> distFn)
    {
      var r = tileSize/2;
      using(var output = new PngImageOutput(tileSize , tileSize)){
        for (int x = 0; x < tileSize; x++) {
          for (int y = 0; y < tileSize; y++) {
            var dist = distFn(x,y);
            var c = 0xff*(r-dist)/r;
            var alpha = dist < r ? c : 0x0;
            output.SetPixel(x,y,0xff, 0xff, 0xff, (int)alpha);
          }
        }
        output.Save(path); 
      }
    }

    public void PaintCorner(String path, Point point)
    {
      var r = tileSize / 2;
      using(var output = new PngImageOutput(tileSize , tileSize)){
        for (int x = 0; x < tileSize; x++) {
          for (int y = 0; y < tileSize; y++) {
            var dist = Point.Distance(point.X*tileSize, point.Y*tileSize, x, y);
            var c = 0xff*(r-dist)/r;
            var alpha = dist < r ? c : 0x0;
            output.SetPixel(x,y,0xff, 0xff, 0xff, (int)alpha);
          }
        }
        output.Save(path); 
      }
    }
  }
}
