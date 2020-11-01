using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimuation
{
    public class MotorBoat : Boat
    {
        public int Horsepower { get; set; }

        public MotorBoat()
        {
            Id = Utils.GenerateID("MB");
            Size = 1;
            DockedTo = new List<int>(Size);
            DaysBeforeDeparture = 3;
            Weight = Utils.RandomNumberInRange(200, 3000);
            MaxSpeedKnots = Utils.RandomNumberInRange(0, 60);
            Horsepower = Utils.RandomNumberInRange(10, 1000);

        }

    }
}
