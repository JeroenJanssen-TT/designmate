using System.Collections.Generic;
using Rhino.Geometry;

public class Floor
{
    public List<Brep> Windows { get; set; }
    public List<Brep> Walls { get; set; }
    public List<Brep> Columns { get; set; }
    public List<Brep> Slab { get; set; }
    public List<Brep> Beams { get; set; }

    public int LevelIndex { get; set; }

    public Floor(int index, List<Brep> windows, List<Brep> walls, List<Brep> columns, List<Brep> slab, List<Brep> beams)
    {
        LevelIndex = index;
        Windows = new List<Brep>(windows);
        Walls = new List<Brep>(walls);
        Columns = new List<Brep>(columns);
        Slab = new List<Brep>(slab);
        Beams = new List<Brep>(beams);
    }
}
