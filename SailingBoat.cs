using System;
using System.Collections.Generic;
using System.Text;

namespace ShippingPort
{
    class SailingBoat : Boat
    {
        public int Length { get; set; }

        public SailingBoat (string idName, int weight, int topSpeed, int length, int daysInPort = 4, int boatLength = 2) : base (idName, weight, topSpeed, daysInPort, boatLength)
        {
            Length = length;
        }
        public override int GetUniqueProp()
        {
            return Length;
        }
    }
}
