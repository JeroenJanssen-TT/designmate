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
        public double MaxHeight { get; set; }

        public BuildingModel(int numFloors, int setback, int facadeType, bool addRoofGarden, List<Floor> inputFloors, double ftf, double maxHeight)
        {
            NumFloors = numFloors;
            Setback = setback;
            FacadeType = facadeType;
            AddRoofGarden = addRoofGarden;
            InputFloors = inputFloors;
            Ftf = ftf;
            MaxHeight = maxHeight;
        }

        public List<Floor> Generate()
        {
            List<Floor> processedFloors = new List<Floor>();

            Floor lastFloor = InputFloors.LastOrDefault();

            Floor currentFloor = lastFloor;
            if((NumFloors + InputFloors.Count)*Ftf>MaxHeight)
                NumFloors = (int)Math.Floor(MaxHeight / Ftf) - InputFloors.Count;

            for (int i = 0; i < NumFloors; i++)
            {
                List<Brep> facadePlaceholder = new List<Brep>();
                foreach (var w in currentFloor.FacadePlaceholder)
                    facadePlaceholder.Add((Brep)w.Duplicate());

                List<Brep> core = new List<Brep>();
                foreach (var c in currentFloor.Core)
                    core.Add((Brep)c.Duplicate());

                List<Brep> columns = new List<Brep>();
                foreach (var c in currentFloor.Columns)
                    columns.Add((Brep)c.Duplicate());

                List<Brep> slab = new List<Brep>();
                foreach (var s in currentFloor.Slab)
                    slab.Add((Brep)s.Duplicate());

                List<Brep> beams = new List<Brep>();
                foreach (var cb in currentFloor.Beams)
                    beams.Add((Brep)cb.Duplicate());

                Transform moveUp = Transform.Translation(0, 0, Ftf);

                facadePlaceholder.ForEach(b => b.Transform(moveUp));
                core.ForEach(b => b.Transform(moveUp));
                columns.ForEach(b => b.Transform(moveUp));
                slab.ForEach(b => b.Transform(moveUp));
                beams.ForEach(b => b.Transform(moveUp));

                //if (Setback > 0)
                //{
                //     pick a wall - translate it inward
                //    Point3d centroid = AreaMassProperties.Compute(slab.First()).Centroid;
                //    Transform scale = Transform.Scale(centroid, 1.0 - (Setback * 0.1));
                //    core.ForEach(b => b.Transform(scale));
                //    slab.ForEach(b => b.Transform(scale));
                //}

                Floor newFloor = new Floor(i, facadePlaceholder, columns, slab, beams, core);

                // if roog garden just increase the load and change color visually?
                if (AddRoofGarden && i == NumFloors - 1)
                {
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
