using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimuation
{
    public class MotorBoat : Boat
    {
        public MotorBoat()
        {
            ID = Utils.GenerateID("MB");
            Size = 1;

        }

    }
}
