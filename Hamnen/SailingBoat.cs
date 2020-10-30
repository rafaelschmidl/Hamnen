using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimuation
{
    public class SailingBoat : Boat
    {
        public SailingBoat()
        {
            Id = Utils.GenerateID("SB");
            Size = 2;
        }
    }
}
