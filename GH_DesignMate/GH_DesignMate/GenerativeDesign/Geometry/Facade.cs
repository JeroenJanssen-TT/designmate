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
        public double CarbonFootprint { get; set; }

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
            Load = 0;
            CarbonFootprint = 0;

            //GET LOAD AND CARBON FOOTPRINT
            //Get load (kN)

            double windowdensity = 2500;
            double brickdensity = 1800;
            double timberdensity = 800;
            double aluminumdensity = 2700;
            double averagedensity = 1700;

            //Get CarbonFoorpint kg CO₂e/kg
            double windowcarbon = 1.3;
            double brickcarbon = 0.24;
            double timbercarbon = 0.46;
            double aluminumcarbon = 2.5;
            double averagecarbon = 1.4;


            foreach (Brep brep in windows) 
            {
                double Vl = brep.GetVolume();
                double mss = Vl * aluminumdensity;
                Load += mss * windowdensity * 0.01;
                CarbonFootprint += mss * windowcarbon;

            }

            foreach (Brep br in solid)
            {
                double Vol = br.GetVolume();
                if (facadetype == "Cladding Metal" || facadetype == "Curtain Wall")
                {
                    double mass = Vol * aluminumdensity;
                    Load += mass * 0.01;
                    CarbonFootprint += mass * aluminumcarbon;
                }

                else if (facadetype == "Cladding Timber")
                {
                    double mass = Vol * timberdensity;
                    Load += mass * 0.01;
                    CarbonFootprint += mass * timbercarbon;
                }

                else if (facadetype == "Bricks")
                {
                    double mass = Vol * brickdensity;
                    Load += mass * 0.01;
                    CarbonFootprint += mass * brickcarbon;
                }

                else
                {
                    double mass = Vol * averagedensity;
                    Load += mass* 0.01;
                    CarbonFootprint += mass * averagecarbon;
                }

            }


        }

    }

    

}
