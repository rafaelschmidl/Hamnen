using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace HarborSimuation
{
    public class Boat
    {
        public string ID { get; set; }
        public int DaysBeforeDeparture { get; set; }
        public int Weight { get; set; }
        public double Size { get; set; }
        public int MaxSpeedKnots { get; set; }

    }
}
