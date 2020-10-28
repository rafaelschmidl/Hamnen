using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimuation
{
    public class Dock
    {
        public bool IsOccupied => OccupiedBy != null || (OccupiedBy == null && OtherRowingBoat != null);
        public bool HasPlaceForeSecondRowingBoat =>
            (OccupiedBy is RowingBoat && OtherRowingBoat == null) || (OccupiedBy == null && OtherRowingBoat is RowingBoat);
        public Boat OccupiedBy { get; set; }
        public RowingBoat OtherRowingBoat { get; set; }

        public int DockNumber { get; set; }

        public Dock(int dockNumber)
        {
            DockNumber = dockNumber;
        }
    }
}
