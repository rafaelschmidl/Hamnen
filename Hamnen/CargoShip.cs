using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimuation
{
    public class CargoShip : Boat
    {
        public int ContainersOnboard { get; set; }

        public CargoShip()
        {
            Id = Utils.GenerateID("CS");
            Size = 4;
            Weight = Utils.RandomNumberInRange(3000, 20000);
            MaxSpeedKnots = Utils.RandomNumberInRange(0, 20);
            ContainersOnboard = Utils.RandomNumberInRange(0, 500);
            DaysBeforeDeparture = 6;

        }
    }
}
