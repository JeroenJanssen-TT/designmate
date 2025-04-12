using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GH_DesignMate.GenerativeDesign.Geometry
{
    public class Building
    {
        List<Floor> Floors { get; set; }
        public Building(List<Floor> floors)
        {
            Floors = floors;
        }
    }
}
