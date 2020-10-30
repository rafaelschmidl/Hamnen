using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimuation
{
    public class Catamaran : Boat
    {
        public Catamaran()
        {
            Id = Utils.GenerateID("CM");
            Size = 3;
        }
    }
}
