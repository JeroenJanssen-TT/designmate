using System;
using System.Collections.Generic;
using GH_DesignMate.GenerativeDesign.Geometry;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GH_DesignMate.GenerativeDesign.Components
{
    public class CreateFacade : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public CreateFacade()
          : base("CreateFacade", "FacadeMaker",
              "Creates a list of Facade objects from categorized Breps",
              "DesignMate", "Generative Design")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Windows", "windows", "Window geometry, grouped by floor", GH_ParamAccess.list);
            pManager.AddBrepParameter("Solid", "solid", "Solid facade geometry, grouped by floor", GH_ParamAccess.list);
            pManager.AddBrepParameter("Slabs", "slabs", "Slab geometry, grouped by floor", GH_ParamAccess.list);
            pManager.AddTextParameter("Facade Type", "facade type", "Facade type", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Facade", "facade", "List of Facade objects assembled from input Breps", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Brep> windows = new List<Brep>();
            List<Brep> solids = new List<Brep>();
            List<Brep> slabs = new List<Brep>();
            string type = string.Empty;

            if (!DA.GetDataList(0, windows)) return;
            if (!DA.GetDataList(1, solids)) return;
            if (!DA.GetDataList(2, slabs)) return;
            if (!DA.GetData(3, ref type)) return;


            // Determine number of floors by slab count (safest reference)
            int numFloors = slabs.Count;

            List<Facade> facades = new List<Facade>();

            for (int i = 0; i < numFloors; i++)
            {
                // Optional: filter elements per floor, or assume uniform subdivision
                // Here we assume all lists are in order and equally divided

                Facade fc = new Facade(i,
                    new List<Brep> { solids.Count > i ? solids[i] : null },
                    new List<Brep> { windows.Count > i ? windows[i] : null },
                    type);
                    

                facades.Add(fc);
            }

            DA.SetDataList(0, facades);


        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9ECA5611-C80F-45E7-84CA-F0CC2864A120"); }
        }
    }
}