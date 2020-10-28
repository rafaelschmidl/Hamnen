using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimuation
{
    public class CargoShip : Boat
    {
        public CargoShip()
        {
            ID = Utils.GenerateID("CS");
            Size = 4;
        }
    }
}
