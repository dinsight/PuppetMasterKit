using System;
namespace PuppetMasterKit.AI.Rules
{
  public class Fact
  {
    public float Grade {get; set;}

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Inference.Fact"/> class.
    /// </summary>
    /// <param name="grade">Grade.</param>
    public Fact(float grade = 0)
    {
      this.Grade = grade;
    }

    /// <summary>
    /// Serves as a hash function for a <see cref="T:PuppetMasterKit.AI.Inference.Fact"/> object.
    /// </summary>
    /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
    public override int GetHashCode()
    {
      return this.GetType().Name.GetHashCode();
    }

    /// <summary>
    /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="T:PuppetMasterKit.AI.Inference.Fact"/>.
    /// </summary>
    /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="T:PuppetMasterKit.AI.Inference.Fact"/>.</param>
    /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current
    /// <see cref="T:PuppetMasterKit.AI.Inference.Fact"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
      if (obj is Fact) {
        return this == (Fact)obj;
      }
      return false;
    }

    /// <summary>
    /// Determines whether a specified instance of <see cref="PuppetMasterKit.AI.Rules.Fact"/> is equal to another
    /// specified <see cref="PuppetMasterKit.AI.Rules.Fact"/>.
    /// </summary>
    /// <param name="lhs">The first <see cref="PuppetMasterKit.AI.Rules.Fact"/> to compare.</param>
    /// <param name="rhs">The second <see cref="PuppetMasterKit.AI.Rules.Fact"/> to compare.</param>
    /// <returns><c>true</c> if <c>lhs</c> and <c>rhs</c> are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Fact lhs, Fact rhs)
    {
      if (Object.ReferenceEquals(lhs, rhs))
        return true;
      
      return lhs.GetHashCode() == rhs.GetHashCode();
    }

    /// <summary>
    /// Determines whether a specified instance of <see cref="PuppetMasterKit.AI.Rules.Fact"/> is not equal to
    /// another specified <see cref="PuppetMasterKit.AI.Rules.Fact"/>.
    /// </summary>
    /// <param name="lhs">The first <see cref="PuppetMasterKit.AI.Rules.Fact"/> to compare.</param>
    /// <param name="rhs">The second <see cref="PuppetMasterKit.AI.Rules.Fact"/> to compare.</param>
    /// <returns><c>true</c> if <c>lhs</c> and <c>rhs</c> are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Fact lhs, Fact rhs)
    {
      return !(lhs == rhs);
    }
  }
}
