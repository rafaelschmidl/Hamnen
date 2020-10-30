using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimuation
{
    public class MotorBoat : Boat
    {
        public MotorBoat()
        {
            Id = Utils.GenerateID("MB");
            Size = 1;

        }

    }
}
