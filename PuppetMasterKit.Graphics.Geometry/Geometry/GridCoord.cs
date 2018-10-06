using System;
namespace PuppetMasterKit.Graphics.Geometry
{
  public class GridCoord
  {
    public int Row { get; private set; }
    public int Col { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Graphics.Geometry.GridCoord"/> class.
    /// </summary>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    public GridCoord(int row, int col)
    {
      Row = row;
      Col = col;
    }

    /// <summary>
    /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="T:PuppetMasterKit.Graphics.Geometry.GridCoord"/>.
    /// </summary>
    /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="T:PuppetMasterKit.Graphics.Geometry.GridCoord"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current
    /// <see cref="T:PuppetMasterKit.Graphics.Geometry.GridCoord"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
      var coord = obj as GridCoord;
      return coord != null &&
             Row == coord.Row &&
             Col == coord.Col;
    }

    /// <summary>
    /// Serves as a hash function for a <see cref="T:PuppetMasterKit.Graphics.Geometry.GridCoord"/> object.
    /// </summary>
    /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data 
    /// structures such as a hash table.</returns>
    public override int GetHashCode()
    {
      var hashCode = 1084646500;
      hashCode = hashCode * -1521134295 + Row.GetHashCode();
      hashCode = hashCode * -1521134295 + Col.GetHashCode();
      return hashCode;
    }

    /// <summary>
    /// Determines whether a specified instance of <see cref="PuppetMasterKit.Graphics.Geometry.GridCoord"/> is equal to
    /// another specified <see cref="PuppetMasterKit.Graphics.Geometry.GridCoord"/>.
    /// </summary>
    /// <param name="lhs">The first <see cref="PuppetMasterKit.Graphics.Geometry.GridCoord"/> to compare.</param>
    /// <param name="rhs">The second <see cref="PuppetMasterKit.Graphics.Geometry.GridCoord"/> to compare.</param>
    /// <returns><c>true</c> if <c>lhs</c> and <c>rhs</c> are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(GridCoord lhs, GridCoord rhs)
    {
      if (lhs is null && rhs is null) {
        return true;
      }
      if (lhs is null || rhs is null)
        return false;
      return lhs.Row == rhs.Row && lhs.Col == rhs.Col;
    }

    /// <summary>
    /// Determines whether a specified instance of <see cref="PuppetMasterKit.Graphics.Geometry.GridCoord"/> is not
    /// equal to another specified <see cref="PuppetMasterKit.Graphics.Geometry.GridCoord"/>.
    /// </summary>
    /// <param name="lhs">The first <see cref="PuppetMasterKit.Graphics.Geometry.GridCoord"/> to compare.</param>
    /// <param name="rhs">The second <see cref="PuppetMasterKit.Graphics.Geometry.GridCoord"/> to compare.</param>
    /// <returns><c>true</c> if <c>lhs</c> and <c>rhs</c> are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(GridCoord lhs, GridCoord rhs)
    {
      return !(lhs == rhs);
    }
  }
}
