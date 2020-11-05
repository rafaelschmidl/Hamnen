using System.Collections.Generic;

namespace HarborSimuation
{
    public class Catamaran : Boat
    {
        public Catamaran()
        {
            Id = Utils.GenerateID("CM");
            Size = 3;
            DockedTo = new List<int>(Size);
            DaysBeforeDeparture = 3;
            Weight = Utils.RandomNumberInRange(1200, 8000);
            MaxSpeedKnots = Utils.RandomNumberInRange(0, 12);
            UniqueProperty = Utils.RandomNumberInRange(1, 4);
        }
    }
}
