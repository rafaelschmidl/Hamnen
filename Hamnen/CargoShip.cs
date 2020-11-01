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
            DockedTo = new List<int>(Size);
            DaysBeforeDeparture = 6;
            Weight = Utils.RandomNumberInRange(3000, 20000);
            MaxSpeedKnots = Utils.RandomNumberInRange(0, 20);
            ContainersOnboard = Utils.RandomNumberInRange(0, 500);

        }
    }
}
