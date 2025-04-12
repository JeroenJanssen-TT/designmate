using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace GH_DesignMate.GenerativeDesign.Components
{
    public class GenerateGeomComp : GH_Component
    {
        public GenerateGeomComp()
          : base("GenerateGeomComp", "GenerateGeomComp",
              "Processes floor geometry for a generative renovation",
              "DesignMate", "Generative Design")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Number of Floors", "numFloors", "Total number of floors to generate", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Setback (m)", "setback", "Setback from facade in meters", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Facade Type", "facadeType", "0 = Glass, 1 = Timber, etc.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Add Roof Garden", "roofGarden", "Whether to include a roof garden", GH_ParamAccess.item);
            pManager.AddGenericParameter("Floor List", "floors", "List of preconfigured floor objects", GH_ParamAccess.list);
            pManager.AddNumberParameter("FtF", "ftf", "Floor to floor height", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Walls", "walls", "Wall geometry from all floors", GH_ParamAccess.list);
            pManager.AddBrepParameter("Columns", "columns", "Column geometry from all floors", GH_ParamAccess.list);
            pManager.AddBrepParameter("Slabs", "slabs", "Slab geometry from all floors", GH_ParamAccess.list);
            pManager.AddBrepParameter("Ceilings", "ceilings", "Ceiling geometry from all floors", GH_ParamAccess.list);
            pManager.AddBrepParameter("Windows", "windows", "Window geometry from all floors", GH_ParamAccess.list);
            pManager.AddBrepParameter("Beams", "beams", "Beam geometry from all floors", GH_ParamAccess.list); // NEW
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int numFloors = 0;
            int setback = 0;
            int facadeType = 0;
            bool addRoofGarden = false;
            List<Floor> inputFloors = new List<Floor>();
            double ftf = 0.0;

            if (!DA.GetData(0, ref numFloors)) return;
            if (!DA.GetData(1, ref setback)) return;
            if (!DA.GetData(2, ref facadeType)) return;
            if (!DA.GetData(3, ref addRoofGarden)) return;
            if (!DA.GetDataList(4, inputFloors)) return;
            if (!DA.GetData(5, ref ftf)) return;

            BuildingModel model = new BuildingModel(numFloors, setback, facadeType, addRoofGarden, inputFloors, ftf);

            List<Floor> generatedFloors = model.Generate();

            List<Brep> allWalls = new List<Brep>();
            List<Brep> allColumns = new List<Brep>();
            List<Brep> allSlabs = new List<Brep>();
            List<Brep> allCeilings = new List<Brep>();
            List<Brep> allWindows = new List<Brep>();
            List<Brep> allBeams = new List<Brep>();

            foreach (Floor f in generatedFloors)
            {
                allWalls.AddRange(f.Walls);
                allColumns.AddRange(f.Columns);
                allSlabs.AddRange(f.Slab);
                allCeilings.AddRange(f.Ceiling);
                allWindows.AddRange(f.Windows);
                allBeams.AddRange(f.Beams);
            }

            DA.SetDataList(0, allWalls);
            DA.SetDataList(1, allColumns);
            DA.SetDataList(2, allSlabs);
            DA.SetDataList(3, allCeilings);
            DA.SetDataList(4, allWindows);
            DA.SetDataList(5, allBeams);
        }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("2D25AEC9-372B-4AAB-B5FC-E7B0B46B4DF2");
    }
}
