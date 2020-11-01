using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimuation
{
    public class Catamaran : Boat
    {
        public int Beds { get; set; }

        public Catamaran()
        {
            Id = Utils.GenerateID("CM");
            Size = 3;
            DockedTo = new List<int>(Size);
            DaysBeforeDeparture = 3;
            Weight = Utils.RandomNumberInRange(1200, 8000);
            MaxSpeedKnots = Utils.RandomNumberInRange(0, 12);
            Beds = Utils.RandomNumberInRange(1, 4);
        }
    }
}
