using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimuation
{
    public class Dock
    {
        public int IdNumber { get; set; }
        public Boat OccupiedBy { get; set; }
        public RowingBoat OtherRowingBoat { get; set; }

        public Dock() { }

        public Dock(int idNumber)
        {
            IdNumber = idNumber;
        }

        public bool IsOccupied()
        {
            if (OccupiedBy != null || OtherRowingBoat != null)
                return true;
            else
                return false;
        }

        public bool HasPlaceForSecondRowingBoat()
        {
            if (OccupiedBy == null && OtherRowingBoat == null)
                return false;
            else if ((OccupiedBy.Id.Substring(0, 2) == "RB" && OtherRowingBoat == null) || (OccupiedBy == null && OtherRowingBoat != null))
                return true;
            else
                return false;
        }

        public void DockBoat(Boat boat)
        {
            OccupiedBy = boat;
        }

        public void DepartBoat()
        {
            OccupiedBy = null;
        }

        public void DockSecondRowingBoat(RowingBoat rowingBoat)
        {
            if (OccupiedBy != null)
                OtherRowingBoat = rowingBoat;
            else if (OtherRowingBoat != null)
                OccupiedBy = rowingBoat;
        }





    }
}
