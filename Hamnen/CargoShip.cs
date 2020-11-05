using System.Collections.Generic;

namespace HarborSimuation
{
    public class CargoShip : Boat
    {
        public CargoShip()
        {
            Id = Utils.GenerateID("CS");
            Size = 4;
            DockedTo = new List<int>(Size);
            DaysBeforeDeparture = 6;
            Weight = Utils.RandomNumberInRange(3000, 20000);
            MaxSpeedKnots = Utils.RandomNumberInRange(0, 20);
            UniqueProperty = Utils.RandomNumberInRange(0, 500);
        }
    }
}
