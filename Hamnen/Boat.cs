using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace HarborSimuation
{
    public class Boat
    {
        public string Id { get; set; }
        public int Size { get; set; }
        public List<int> DockedTo { get; set; }
        public int DaysBeforeDeparture { get; set; }
        public int Weight { get; set; }
        public int MaxSpeedKnots { get; set; }
        public int UniqueProperty { get; set; }
    }
}
