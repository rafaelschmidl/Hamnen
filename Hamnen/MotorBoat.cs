using System.Collections.Generic;

namespace HarborSimuation
{
    public class MotorBoat : Boat
    {
        public MotorBoat()
        {
            Id = Utils.GenerateID("MB");
            Size = 1;
            DockedTo = new List<int>(Size);
            DaysBeforeDeparture = 3;
            Weight = Utils.RandomNumberInRange(200, 3000);
            MaxSpeedKnots = Utils.RandomNumberInRange(0, 60);
            UniqueProperty = Utils.RandomNumberInRange(10, 1000);
        }
    }
}
