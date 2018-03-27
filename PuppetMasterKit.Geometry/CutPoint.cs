using System;
namespace PuppetMasterKit.Geometry
{
    class CutPoint
    {
        public Point Point { get; private set;} 

        public bool IsWaypoint { get; set;}

        public bool IsVisited { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Geometry.CutPoint"/> class.
        /// </summary>
        public CutPoint(Point point, bool isWaypoint = false, bool isVisited = false)
        {
            this.Point = point;
            this.IsWaypoint = isWaypoint;
            this.IsVisited = isVisited;
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="lhs">The first <see cref="PuppetMasterKit.Geometry.CutPoint"/> to compare.</param>
        /// <param name="rhs">The second <see cref="PuppetMasterKit.Geometry.CutPoint"/> to compare.</param>
        /// <returns><c>true</c> if <c>lhs</c> and <c>rhs</c> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(CutPoint lhs, CutPoint rhs) 
        {
            if ( Object.ReferenceEquals(lhs, null) 
              && Object.ReferenceEquals(rhs, null))
                return true;

            if (!Object.ReferenceEquals(lhs, null)
              && !Object.ReferenceEquals(rhs, null))
                return lhs.Point == rhs.Point;
            
            return false;
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        /// <param name="lhs">The first <see cref="PuppetMasterKit.Geometry.CutPoint"/> to compare.</param>
        /// <param name="rhs">The second <see cref="PuppetMasterKit.Geometry.CutPoint"/> to compare.</param>
        /// <returns><c>true</c> if <c>lhs</c> and <c>rhs</c> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(CutPoint lhs, CutPoint rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Equality method
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="T:PuppetMasterKit.Geometry.CutPoint"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current
        /// <see cref="T:PuppetMasterKit.Geometry.CutPoint"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
            if (obj is CutPoint)
            {
                return this.Point == ((CutPoint)obj).Point;
            }
            return false;
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="T:PuppetMasterKit.Geometry.CutPoint"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
		public override int GetHashCode()
		{
            return base.GetHashCode();
		}
	}
}
