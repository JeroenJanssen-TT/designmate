using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace GH_DesignMate.GenerativeDesign
{
    public class GenerateGeomComp : GH_Component
    {
        public GenerateGeomComp()
          : base("MyComponent1", "Nickname",
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
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Processed Floors", "processed", "Resulting floor data with modifications", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int numFloors = 0;
            int setback = 0;
            int facadeType = 0;
            bool addRoofGarden = false;
            List<Floor> inputFloors = new List<Floor>();

            if (!DA.GetData(0, ref numFloors)) return;
            if (!DA.GetData(1, ref setback)) return;
            if (!DA.GetData(2, ref facadeType)) return;
            if (!DA.GetData(3, ref addRoofGarden)) return;
            if (!DA.GetDataList(4, inputFloors)) return;

            BuildingModel model = new BuildingModel(numFloors, setback, facadeType, addRoofGarden, inputFloors);

            List<Floor> generatedFloors = model.Generate();

            DA.SetDataList(0, generatedFloors);
        }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("2D25AEC9-372B-4AAB-B5FC-E7B0B46B4DF2");
    }
}
