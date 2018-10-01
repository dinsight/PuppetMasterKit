using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrainGen
{
  public class Bicubic
  {
    private Gradient gradient;

    /// <summary>
    /// 
    /// </summary>
    public Gradient Gradient { get => gradient; set => gradient = value; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gradient"></param>
    public Bicubic(Gradient gradient) {
      this.Gradient = gradient;
    }

    private float CubicInterpolate(float []p, float x) {
     return (float)(p[1] + 0.5 * x*(p[2] - p[0] + x*(2.0*p[0] - 5.0*p[1] + 4.0*p[2] - p[3] + x*(3.0*(p[1] - p[2]) + p[3] - p[0]))));
    }

    private float BicubicInterpolate (float [][]p, float x, float y) {
	    var arr = new float[4];
	    arr[0] = CubicInterpolate(p[0], x);
	    arr[1] = CubicInterpolate(p[1], x);
	    arr[2] = CubicInterpolate(p[2], x);
	    arr[3] = CubicInterpolate(p[3], x);
	    return CubicInterpolate(arr, y);
    }
    
    int Clamp(int x)
    {
      if(x<0) 
        return 0;
      else if(x>gradient.MaxFrequency-1) 
        return gradient.MaxFrequency-1;
      return x;
    }
    
    private int currXc = -1;
    private int currYc = -1;
    private float[][] p = null;
    public float Noise(float x, float y) 
    {
      int xc = (int)x;
      int yc = (int)y;

      if(xc != currXc || yc != currYc || p == null){ 
        currXc = xc;
        currYc = yc;
        p = new float[][]{
          new float[]{gradient[Clamp(yc-1),Clamp(xc-1)],
                      gradient[Clamp(yc-1),Clamp(xc)],
                      gradient[Clamp(yc-1),Clamp(xc+1)],
                      gradient[Clamp(yc-1),Clamp(xc+2)] },
          new float[]{gradient[Clamp(yc),Clamp(xc-1)],
                      gradient[Clamp(yc),Clamp(xc)],
                      gradient[Clamp(yc),Clamp(xc+1)],
                      gradient[Clamp(yc),Clamp(xc+2)] },
          new float[]{gradient[Clamp(yc+1),Clamp(xc-1)],
                      gradient[Clamp(yc+1),Clamp(xc)],
                      gradient[Clamp(yc+1),Clamp(xc+1)],
                      gradient[Clamp(yc+1),Clamp(xc+2)] },
          new float[]{gradient[Clamp(yc+2),Clamp(xc-1)],
                      gradient[Clamp(yc+2),Clamp(xc)],
                      gradient[Clamp(yc+2),Clamp(xc+1)],
                      gradient[Clamp(yc+2),Clamp(xc+2)] },
        };
      }
      
      return BicubicInterpolate(p, x-xc, y-yc);
    }
  }
}
