using System;
using System.Collections.Generic;
using System.Text;

namespace ShippingPort
{
    public class RowBoat : Boat
    {
        public int MaxPassenger { get; set; }

        public RowBoat (string idName, int weight, int topSpeed, int maxPassenger, int daysInPort = 1, int boatLength = 1) : base (idName, weight, topSpeed, daysInPort, boatLength)
        {
            MaxPassenger = maxPassenger;
        }
        public override int GetUniqueProp()
        {
            return MaxPassenger;
        }
    }
}
