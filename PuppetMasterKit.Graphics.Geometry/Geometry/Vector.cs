using System;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.Graphics.Geometry
{
    public class Vector
    {
        public static Vector Zero = new Vector(0, 0);

        public float Dx { get; set; }

        public float Dy { get; set; }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="T:PuppetMasterKit.Geometry.Vector"/> class.
        /// </summary>
        /// <param name="dx">Dx.</param>
        /// <param name="dy">Dy.</param>
        public Vector(float dx, float dy)
        {
            this.Dx = dx;
            this.Dy = dy;
        }

        /// <summary>
        /// Calcualtes the magnitude of this vector
        /// </summary>
        /// <returns>The magnitude.</returns>
        public float Magnitude()
        {
            return (float)Math.Sqrt(Dx * Dx + Dy * Dy);
        }

        /// <summary>
        /// Unit vector out of the current vector
        /// </summary>
        /// <returns>The unit.</returns>
        public Vector Unit()
        {
            if (this==Zero)
            {
                return this;
            }
            return this / Magnitude();
        }

        /// <summary>
        /// Truncates the current vector 
        /// </summary>
        /// <returns>The truncate.</returns>
        /// <param name="max">Max.</param>
        public Vector Truncate(float max) 
        {
            if (Magnitude() > max)
            {
                return Unit() * max;
            }

            return this;
        }

        /// <summary>
        /// Convert to position
        /// </summary>
        /// <returns>The position.</returns>
        public Point ToPosition()
        {
            return new Point(Dx, Dy);
        }

        /// <summary>
        /// Clone the current vector
        /// </summary>
        /// <returns>The clone.</returns>
        public Vector Clone()
        {
            return new Vector(Dx, Dy);
        }

        /// <summary>
        /// Set the vector's angle
        /// </summary>
        /// <returns>The angle.</returns>
        /// <param name="angle">Angle.</param>
        public Vector SetAngle(float angle)
        {
            var len = Magnitude();

            Dx = (float)Math.Cos(angle) * len;

            Dy = (float)Math.Sin(angle) * len;

            return this;
        }

        /// <summary>
        /// Scalar product for a vector
        /// </summary>
        /// <param name="vector">The <see cref="PuppetMasterKit.Geometry.Vector"/> to multiply.</param>
        /// <param name="scalar">The <see cref="float"/> to multiply.</param>
        /// <returns>The <see cref="T:PuppetMasterKit.Geometry.Vector"/> that is the <c>vector</c> * <c>scalar</c>.</returns>
        public static Vector operator*(Vector vector, float scalar)
        {
            return new Vector(vector.Dx * scalar, vector.Dy * scalar);
        }

        /// <summary>
        /// Scalar division
        /// </summary>
        /// <param name="vector">The <see cref="PuppetMasterKit.Geometry.Vector"/> to divide (the divident).</param>
        /// <param name="scalar">The <see cref="float"/> to divide (the divisor).</param>
        /// <returns>The <see cref="T:PuppetMasterKit.Geometry.Vector"/> that is the <c>vector</c> / <c>scalar</c>.</returns>
        public static Vector operator/(Vector vector, float scalar)
        {
            return new Vector(vector.Dx / scalar, vector.Dy / scalar);
        }

        /// <summary>
        /// Vector addition
        /// </summary>
        /// <param name="lhs">The first <see cref="PuppetMasterKit.Geometry.Vector"/> to add.</param>
        /// <param name="rhs">The second <see cref="PuppetMasterKit.Geometry.Vector"/> to add.</param>
        /// <returns>The <see cref="T:PuppetMasterKit.Geometry.Vector"/> that is the sum of the values of <c>lhs</c> and <c>rhs</c>.</returns>
        public static Vector operator +(Vector lhs, Vector rhs)
        {
            return new Vector(lhs.Dx + lhs.Dx, lhs.Dy + rhs.Dy);
        }

        /// <summary>
        /// Vector subtraction
        /// </summary>
        /// <param name="lhs">The <see cref="PuppetMasterKit.Geometry.Vector"/> to subtract from (the minuend).</param>
        /// <param name="rhs">The <see cref="PuppetMasterKit.Geometry.Vector"/> to subtract (the subtrahend).</param>
        /// <returns>The <see cref="T:PuppetMasterKit.Geometry.Vector"/> that is the <c>lhs</c> minus <c>rhs</c>.</returns>
        public static Vector operator -(Vector lhs, Vector rhs)
        {
            return new Vector(lhs.Dx - lhs.Dx, lhs.Dy - rhs.Dy);
        }

        /// <summary>
        /// Vector and point addition
        /// </summary>
        /// <param name="vector">The first <see cref="PuppetMasterKit.Geometry.Vector"/> to add.</param>
        /// <param name="point">The second <see cref="PuppetMasterKit.Geometry.Point"/> to add.</param>
        /// <returns>The <see cref="T:PuppetMasterKit.Geometry.Vector"/> that is the sum of the values of <c>vector</c> and <c>point</c>.</returns>
        public static Vector operator +(Vector vector, Point point)
        {
            return new Vector(vector.Dx + point.X, vector.Dy + point.Y);
        }

        /// <summary>
        /// Point and vector addition
        /// </summary>
        /// <param name="point">The first <see cref="PuppetMasterKit.Geometry.Point"/> to add.</param>
        /// <param name="vector">The second <see cref="PuppetMasterKit.Geometry.Vector"/> to add.</param>
        /// <returns>The <see cref="T:PuppetMasterKit.Geometry.Vector"/> that is the sum of the values of <c>point</c> and <c>vector</c>.</returns>
        public static Vector operator +(Point point, Vector vector)
        {
            return new Vector(vector.Dx + point.X, vector.Dy + point.Y);
        }

        /// <summary>
        /// Subtraction of vector and point
        /// </summary>
        /// <param name="vector">The <see cref="PuppetMasterKit.Geometry.Vector"/> to subtract from (the minuend).</param>
        /// <param name="point">The <see cref="PuppetMasterKit.Geometry.Point"/> to subtract (the subtrahend).</param>
        /// <returns>The <see cref="T:PuppetMasterKit.Geometry.Vector"/> that is the <c>vector</c> minus <c>point</c>.</returns>
        public static Vector operator -(Vector vector, Point point)
        {
            return new Vector(vector.Dx - point.X, vector.Dy - point.Y);
        }

        /// <summary>
        /// Subtraction of point and vector
        /// </summary>
        /// <param name="point">The <see cref="PuppetMasterKit.Geometry.Point"/> to subtract from (the minuend).</param>
        /// <param name="vector">The <see cref="PuppetMasterKit.Geometry.Vector"/> to subtract (the subtrahend).</param>
        /// <returns>The <see cref="T:PuppetMasterKit.Geometry.Vector"/> that is the <c>point</c> minus <c>vector</c>.</returns>
        public static Vector operator -(Point point, Vector vector)
        {
            return new Vector(point.X - vector.Dx, point.Y - vector.Dy);
        }

        /// <summary>
        /// Vector comparison
        /// </summary>
        /// <param name="lhs">The first <see cref="PuppetMasterKit.Geometry.Vector"/> to compare.</param>
        /// <param name="rhs">The second <see cref="PuppetMasterKit.Geometry.Vector"/> to compare.</param>
        /// <returns><c>true</c> if <c>lhs</c> and <c>rhs</c> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Vector lhs, Vector rhs)
        {
            return Math.Abs(lhs.Dx - rhs.Dx) < Float.EPSILON 
                       && Math.Abs(lhs.Dy - rhs.Dy) < Float.EPSILON;
        }

        /// <summary>
        /// Vector comparison
        /// </summary>
        /// <param name="lhs">The first <see cref="PuppetMasterKit.Geometry.Vector"/> to compare.</param>
        /// <param name="rhs">The second <see cref="PuppetMasterKit.Geometry.Vector"/> to compare.</param>
        /// <returns><c>true</c> if <c>lhs</c> and <c>rhs</c> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Vector lhs, Vector rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Equals operator
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="T:PuppetMasterKit.Geometry.Vector"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current
        /// <see cref="T:PuppetMasterKit.Geometry.Vector"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Vector) {
                return this == (Vector)obj;
            }
            return false;
		}

        /// <summary>
        /// Hash code
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
	}
}
