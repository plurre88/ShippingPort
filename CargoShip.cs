using System;
using System.Collections.Generic;
using System.Text;

namespace ShippingPort
{
    class CargoShip : Boat
    {
        public int NumberOfContainers { get; set; }

        public CargoShip(string idName, int weight, int topSpeed, int numberOfContainers, int daysInPort = 6, int boatLength = 4) : base (idName, weight, topSpeed, daysInPort, boatLength)
        {
            NumberOfContainers = numberOfContainers;
        }
        public override int GetUniqueProp()
        {
            return NumberOfContainers;
        }
    }
}
