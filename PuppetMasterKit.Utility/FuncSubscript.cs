using System;
namespace PuppetMasterKit.Utility
{

  //Functional subscript
  //Wraps a function that returns values based on given coordinates
  public class FuncSubscript : I2DSubscript<float>
  {
    private Func<int, int, float> subscript;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Utility.FuncSubscript"/> class.
    /// </summary>
    /// <param name="subscript">Subscript.</param>
    public FuncSubscript(Func<int,int,float> subscript)
    {
      this.subscript = subscript;
    }

    public float this[int row, int col] => subscript(row,col);
  }
}
