using System.Collections.Generic;

namespace HarborSimuation
{
    public class RowingBoat : Boat
    {
        public RowingBoat()
        {
            Id = Utils.GenerateID("RB");
            Size = 1;
            DockedTo = new List<int>(Size);
            DaysBeforeDeparture = 1;
            Weight = Utils.RandomNumberInRange(100, 300);
            MaxSpeedKnots = Utils.RandomNumberInRange(0, 3);
            UniqueProperty = Utils.RandomNumberInRange(1, 6);
        }
    }
}
