using System;
namespace PuppetMasterKit.Utility
{

  //Functional subscript
  //Wraps a function that returns values based on given coordinates
  public class FuncSubscript : I2DSubscript<float?>
  {
    private readonly Func<int, int, float> subscript;
    private readonly int rows;
    private readonly int cols;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Utility.FuncSubscript"/> class.
    /// </summary>
    /// <param name="subscript">Subscript.</param>
    public FuncSubscript(Func<int,int,float> subscript, int rows, int cols)
    {
      this.subscript = subscript;
      this.rows = rows;
      this.cols = cols;
    }

    public float? this[int row, int col] => subscript(row,col);

    public int Rows => rows;

    public int Cols => cols;
  }
}
