using System;
using System.Collections.Generic;
using System.Text;

namespace ShippingPort
{
    class MotorBoat : Boat
    {
        public int HorsePower { get; set; }

        public MotorBoat(string idName, int weight, int topSpeed, int horsePower, int daysInPort = 3, int boatLength = 1) : base (idName, weight, topSpeed, daysInPort, boatLength)
        {
            HorsePower = horsePower;
        }
        public override int GetUniqueProp()
        {
            return HorsePower;
        }
    }
}
