using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rhino.Geometry;

namespace GH_DesignMate.GenerativeDesign.Geometry
{
    public class Facade
    {
        public List<Brep> Windows { get; set; }
        public List<Brep> Solid { get; set; }

        public string FacadeType { get; set; }

        public double Load {  get; set; }

        public int LevelIndex { get; set; }

        public Facade(int index, List<Brep> windows, List<Brep> solid, string facadetype)
        {
            LevelIndex = index;
            Windows = new List<Brep>(windows);
            Solid = new List<Brep>(solid);
            FacadeType = facadetype;

        }

        // Method to generate curtain wall representation
        public void GenerateFacade(int index, List<Brep> solid, List<Brep> windows, string facadetype)
        {
            LevelIndex = index;
            Windows = new List<Brep>(windows);
            Solid = new List<Brep>(solid);
            FacadeType = facadetype;


            //Get load (kg by m3)

            double windowdensity = 2500;
            double brickdensity = 1800;
            double timberdensity = 800;
            double aluminumdensity = 2700;
            double averagedensity = 1700;

            Load = 0;

            foreach(Brep brep in windows) 
            {
                Load += brep.GetVolume() * windowdensity * 0.01;

            }

            foreach (Brep br in solid)
            {
                if (facadetype == "Cladding Metal" || facadetype == "Curtain Wall")
                {
                    Load += br.GetVolume() * aluminumdensity * 0.01;
                }

                else if (facadetype == "Cladding Timber")
                {
                    Load += br.GetVolume() * timberdensity * 0.01;
                }

                else if (facadetype == "Bricks")
                {
                    Load += br.GetVolume() * brickdensity * 0.01;
                }

                else
                {
                    Load += br.GetVolume() * averagedensity* 0.01;
                }

            }

        }

    }

    

}
