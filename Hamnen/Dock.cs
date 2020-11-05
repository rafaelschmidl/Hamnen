namespace Harbor
{
    public class Dock
    {
        public int DockNumber { get; set; }
        public bool IsOccupied { get; set; }
        public bool HasPlaceForAnotherRowingBoat { get; set; }

        public Dock() {}

        public Dock(int dockNumber)
        {
            DockNumber = dockNumber;
            IsOccupied = false;
            HasPlaceForAnotherRowingBoat = true;
        }
    }
}
