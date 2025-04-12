using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace GH_DesignMate
{
    public class GH_DesignMateInfo : GH_AssemblyInfo
    {
        public override string Name => "GH_DesignMate";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("75a2e7e6-3152-48b2-9d28-8fe16f6fd231");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";

        //Return a string representing the version.  This returns the same version as the assembly.
        public override string AssemblyVersion => GetType().Assembly.GetName().Version.ToString();
    }
}