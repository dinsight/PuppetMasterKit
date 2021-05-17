using System;
using System.Collections.Generic;
using PuppetMasterKit.Utility;
using System.Collections.ObjectModel;

namespace PuppetMasterKit.Terrain.Map
{
	public class Contour : IEnumerable<GridCoord>, System.Collections.IEnumerable
	{
    private List<GridCoord> contourLines;

    public enum ContourType
		{
			OUTSIDE,
			INSIDE
		}
    public int ContoursCount => contourLines.Count;
    public ContourType Type { get; private set; }
    public IReadOnlyCollection<GridCoord> ContourLines => new ReadOnlyCollection<GridCoord>(contourLines);
    public GridCoord this[int i] => contourLines[i];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="coords"></param>
    public Contour(ContourType type, List<GridCoord> coords)
    {
      this.contourLines = new List<GridCoord>(coords);
      this.Type = type;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator<GridCoord> GetEnumerator() {
			foreach (var item in contourLines)
			{
				yield return item;
			}
		}

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
