﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMasterKit.Terrain.Map.CellularAutomata
{
  static class Utils
  {
    
    public static int[,] CopyTo(this int[,] map, int[,] to){ 
      var dim_s1 = map.GetLength(0);
      var dim_s2 = map.GetLength(1);
      var dim_d1 = map.GetLength(0);
      var dim_d2 = map.GetLength(1);
      if(dim_s1!=dim_d1 || dim_s2 != dim_d2)
        return null;
      for (int i = 0; i < dim_s1; i++) {
        for (int j = 0; j < dim_s2; j++) {
          to[i,j]=map[i,j];
        }
      }
      return to;
    }

    public static IEnumerable<int> GetNeighbours(this int[,] map, Func<int,bool> criteria, int i, int j, int step=1)
    {
      var dim1 = map.GetLength(0);
      var dim2 = map.GetLength(1);
      //if(i==0 || j==0 || i==dim1-step || j == dim1-step){
      //  for (int ii = 0; ii < 8; ii++) {
      //    yield return ii+1;
      //  }
      //} else 
      {
        if (i - step >= 0 && j - step >= 0 && criteria(map[i - step, j - step])) yield return 1;
        if (i - step >= 0 && criteria(map[i - step, j])) yield return 2;
        if (i - step >= 0 && j + step < dim2 && criteria(map[i - step, j + step])) yield return 3;
        if (j + step < dim2 && criteria(map[i, j + step])) yield return 4;
        if (i + step < dim1 && j + step < dim2 && criteria(map[i + step, j + step])) yield return 5;
        if (i + step < dim1 && criteria(map[i + step, j])) yield return 6;
        if (i + step < dim1 && j - step >= 0 && criteria(map[i + step, j - step])) yield return 7;
        if (j - step >= 0 && criteria(map[i, j - step])) yield return 8;
      }
    }
  }
}
