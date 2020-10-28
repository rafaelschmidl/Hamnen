using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimuation
{
    public class RowingBoat : Boat
    {

        public RowingBoat()
        {
            ID = Utils.GenerateID("RB");
            Size = 0.5;
        }

    }
}
