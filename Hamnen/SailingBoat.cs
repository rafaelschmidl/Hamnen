using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimuation
{
    public class SailingBoat : Boat
    {
        public SailingBoat()
        {
            ID = Utils.GenerateID("SB");
            Size = 2;
        }
    }
}
