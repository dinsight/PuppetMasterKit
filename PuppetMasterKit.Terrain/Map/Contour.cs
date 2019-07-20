using System;
using System.Collections.Generic;
using PuppetMasterKit.Utility;
using System.Collections.ObjectModel;

namespace PuppetMasterKit.Terrain.Map
{
	public class Contour : IEnumerable<GridCoord>, System.Collections.IEnumerable
	{
		public enum ContourType
		{
			OUTSIDE,
			INSIDE
		}
		private List<GridCoord> coords = new List<GridCoord>();
		public ContourType Type { get; private set; }
		public IReadOnlyCollection<GridCoord> Coords
		{
			get
			{
				return new ReadOnlyCollection<GridCoord>(coords);
			}
		}

		public Contour(ContourType type, List<GridCoord> coords)
		{
			this.Type = type;
			this.coords.AddRange(coords);
		}

		public int Count
		{
			get { return coords.Count; }
		}

		public GridCoord this[int i]{
      get{
        return coords[i];
      }
    }

		public IEnumerator<GridCoord> GetEnumerator() {
			foreach (var item in coords)
			{
				yield return item;
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
