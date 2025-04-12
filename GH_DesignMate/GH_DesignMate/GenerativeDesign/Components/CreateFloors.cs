using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GH_DesignMate.GenerativeDesign.Components
{
    public class CreateFloors : GH_Component
    {
        public CreateFloors()
          : base("CreateFloors", "FloorMaker",
              "Creates a list of Floor objects from categorized Breps",
              "DesignMate", "Generative Design")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("FacadePlaceholder", "FacadePlaceholder", "", GH_ParamAccess.list);
            pManager.AddBrepParameter("Core", "Core", "", GH_ParamAccess.list);
            pManager.AddBrepParameter("Columns", "columns", "Column geometry, grouped by floor", GH_ParamAccess.list);
            pManager.AddBrepParameter("Slabs", "slabs", "Slab geometry, grouped by floor", GH_ParamAccess.list);
            pManager.AddBrepParameter("Beams", "beams", "Beam geometry, grouped by floor", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Floors", "floors", "List of Floor objects assembled from input Breps", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Brep> FacadePlaceholder = new List<Brep>();
            List<Brep> core = new List<Brep>();
            List<Brep> columns = new List<Brep>();
            List<Brep> slabs = new List<Brep>();
            List<Brep> beams = new List<Brep>();

            if (!DA.GetDataList(0, FacadePlaceholder)) return;
            if (!DA.GetDataList(1, core)) return;
            if (!DA.GetDataList(2, columns)) return;
            if (!DA.GetDataList(3, slabs)) return;
            if (!DA.GetDataList(4, beams)) return;

            // Determine number of floors by slab count (safest reference)
            int numFloors = slabs.Count;

            List<Floor> floors = new List<Floor>();

            for (int i = 0; i < numFloors; i++)
            {
                // Optional: filter elements per floor, or assume uniform subdivision
                // Here we assume all lists are in order and equally divided

                Floor f = new Floor(i,
                    new List<Brep> { FacadePlaceholder.Count > i ? FacadePlaceholder[i] : null },
                    new List<Brep> { core.Count > i ? core[i] : null },
                    new List<Brep> { columns.Count > i ? columns[i] : null },
                    new List<Brep> { slabs.Count > i ? slabs[i] : null },
                    new List<Brep> { beams.Count > i ? beams[i] : null });

                floors.Add(f);
            }

            DA.SetDataList(0, floors);
        }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("1BF1A050-2FD3-48B2-BDB6-5CD5B3BC4AF1");
    }
}
