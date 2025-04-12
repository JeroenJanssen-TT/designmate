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

        public BuildingModel(int numFloors, int setback, int facadeType, bool addRoofGarden, List<Floor> inputFloors)
        {
            NumFloors = numFloors;
            Setback = setback;
            FacadeType = facadeType;
            AddRoofGarden = addRoofGarden;
            InputFloors = inputFloors;
        }

        public List<Floor> Generate()
        {
            List<Floor> processedFloors = new List<Floor>();

            for (int i = 0; i < NumFloors; i++)
            {

            }

            return processedFloors;
        }
    }

}
