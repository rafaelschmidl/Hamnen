using System.Collections.Generic;

namespace HarborSimuation
{
    public class SailingBoat : Boat
    {
        public SailingBoat()
        {
            Id = Utils.GenerateID("SB");
            Size = 2;
            DockedTo = new List<int>(Size);
            DaysBeforeDeparture = 4;
            Weight = Utils.RandomNumberInRange(800, 6000);
            MaxSpeedKnots = Utils.RandomNumberInRange(0, 12);
            UniqueProperty = Utils.RandomNumberInRange(10, 60);
        }
    }
}
