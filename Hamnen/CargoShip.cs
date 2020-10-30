using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimuation
{
    public class CargoShip : Boat
    {
        public CargoShip()
        {
            Id = Utils.GenerateID("CS");
            Size = 4;
        }
    }
}
