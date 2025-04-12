using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GH_DesignMate.GenerativeDesign
{
    public class BuildingModel
    {
        public int NumFloors { get; set; }
        public int Setback { get; set; }
        public int FacadeType { get; set; }
        public bool AddRoofGarden { get; set; }
        public List<Floor> InputFloors { get; set; }
        public double Ftf { get; set; }

        public BuildingModel(int numFloors, int setback, int facadeType, bool addRoofGarden, List<Floor> inputFloors, double ftf)
        {
            NumFloors = numFloors;
            Setback = setback;
            FacadeType = facadeType;
            AddRoofGarden = addRoofGarden;
            InputFloors = inputFloors;
            Ftf = ftf;
        }

        public List<Floor> Generate()
        {
            List<Floor> processedFloors = new List<Floor>();

            Floor lastFloor = InputFloors.LastOrDefault();

            Floor currentFloor = lastFloor;


            for (int i = 0; i < NumFloors; i++)
            {
                // --- 1. Deep copy the last floor's geometry ---
                List<Brep> windows = new List<Brep>();
                foreach (var w in currentFloor.Windows)
                    windows.Add((Brep)w.Duplicate());

                List<Brep> walls = new List<Brep>();
                foreach (var wall in currentFloor.Walls)
                    walls.Add((Brep)wall.Duplicate());

                List<Brep> columns = new List<Brep>();
                foreach (var c in currentFloor.Columns)
                    columns.Add((Brep)c.Duplicate());

                List<Brep> slab = new List<Brep>();
                foreach (var s in currentFloor.Slab)
                    slab.Add((Brep)s.Duplicate());

                List<Brep> ceiling = new List<Brep>();
                foreach (var c in currentFloor.Ceiling)
                    ceiling.Add((Brep)c.Duplicate());

                List<Brep> beams = new List<Brep>();
                foreach (var cb in currentFloor.Beams)
                    beams.Add((Brep)cb.Duplicate());

                Transform moveUp = Transform.Translation(0, 0, Ftf);

                windows.ForEach(b => b.Transform(moveUp));
                walls.ForEach(b => b.Transform(moveUp));
                columns.ForEach(b => b.Transform(moveUp));
                slab.ForEach(b => b.Transform(moveUp));
                ceiling.ForEach(b => b.Transform(moveUp));
                beams.ForEach(b => b.Transform(moveUp));

                if (Setback > 0)
                {
                    // pick a wall - translate it inward
                    // 
                    // Uniform inward scaling around center (simplified)
                    // Note: Use better local-plane logic if needed
                    Point3d centroid = AreaMassProperties.Compute(slab.First()).Centroid;
                    Transform scale = Transform.Scale(centroid, 1.0 - (Setback * 0.1));
                    walls.ForEach(b => b.Transform(scale));
                    slab.ForEach(b => b.Transform(scale));
                    ceiling.ForEach(b => b.Transform(scale));
                }

                Floor newFloor = new Floor(i, windows, walls, columns, slab, ceiling, beams);

                // --- 5. Optional: Add a roof garden on the top floor ---
                if (AddRoofGarden && i == NumFloors - 1)
                {
                    // Example: tag by adding a Brep surface, or a flag in the future
                    // You can replace this with a green roof Brep or annotation marker
                }

                // Add to result
                processedFloors.Add(newFloor);

                // Update currentFloor for next iteration
                currentFloor = newFloor;
            }

            return processedFloors;
        }
    }

}
