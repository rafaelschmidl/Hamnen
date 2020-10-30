using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimuation
{
    public class RowingBoat : Boat
    {
        public int MaxPassangers { get; }

        public RowingBoat()
        {
            Id = Utils.GenerateID("RB");
            Size = 1;
            Weight = Utils.RandomNumberInRange(100, 300);
            MaxSpeedKnots = Utils.RandomNumberInRange(0, 3);
            MaxPassangers = Utils.RandomNumberInRange(1, 6);
            DaysBeforeDeparture = 1;
        }

    }
}
