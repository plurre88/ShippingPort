using System;
using System.Collections.Generic;
using System.Text;

namespace ShippingPort
{
    public class Boat
    {
        public string IdName { get; set; }
        public int Weight { get; set; }
        public int TopSpeed { get; set; }
        public int DaysInPort { get; set; }
        public int BoatLength { get; set; }

        public Boat (string idName, int weight, int topSpeed, int daysInPort, int boatLength)
        {
            IdName = idName;
            Weight = weight;
            TopSpeed = topSpeed;
            DaysInPort = daysInPort;
            BoatLength = boatLength;
        }
        public virtual int GetUniqueProp()
        {
            return 0;
        }
    }
}
