using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using Harbor;

namespace HarborSimuation
{
    public class Harbor
    {
        public List<Boat> DockedBoats { get; set; }
        public List<Dock> DocksLeft { get; }
        public List<Dock> DocksRight { get; }


        public Harbor()
        {
            DockedBoats = new List<Boat>();

            DocksLeft = new List<Dock>(32);
            DocksRight = new List<Dock>(32);

            for (int i = 0; i < 32; i++)
            {
                DocksLeft.Add(new Dock(i + 1));
                DocksRight.Add(new Dock(i + 32 + 1));
            }



        }

        public void NextDay(int numberOfIncomingBoats)
        {

            DecrementDaysBeforeDeparture();
            DepartBoats();
            DockIncomingBoats(GenerateIncomingBoats(numberOfIncomingBoats));
        }

        private void DecrementDaysBeforeDeparture()
        {
            DockedBoats.ForEach(boat => boat.DaysBeforeDeparture--);
        }

        private void DepartBoats()
        {
            List<Boat> departingBoats = DockedBoats.Where(boat => boat.DaysBeforeDeparture < 0).ToList();

            departingBoats.ForEach(boat => {
                boat.DockedTo.ForEach(dockNumber => {
                    if (dockNumber <= DocksLeft.Count)
                        DocksLeft[dockNumber - 1].IsOccupied = false;
                    else
                        DocksRight[dockNumber - DocksLeft.Count - 1].IsOccupied = false;
                });
            });
            DockedBoats.RemoveAll(b => b.DaysBeforeDeparture < 0);
        }

        private void DockIncomingBoats(List<Boat> incomingBoats)
        {
            incomingBoats.ForEach(boat =>
            {
                if (boat.Size > 2)
                    DockBoat(boat, DocksLeft, DocksRight);
                else
                    DockBoat(boat, DocksRight, DocksLeft);
            });
        }

        private void DockBoat(Boat boat, List<Dock> primaryDocks, List<Dock> secondaryDocks)
        {
            bool canDock = false;

            for (int i = 0; i < primaryDocks.Count; i++)
            {
                if (!primaryDocks[i].IsOccupied && i + boat.Size < primaryDocks.Count)
                {
                    canDock = true;

                    for (int j = 1; j < boat.Size; j++)
                        if (primaryDocks[i + j].IsOccupied)
                            canDock = false;

                    if (canDock)
                    {
                        for (int k = 0; k < boat.Size; k++)
                        {
                            primaryDocks[i + k].IsOccupied = true;
                            boat.DockedTo.Add(primaryDocks[i + k].DockNumber);
                        }
                        DockedBoats.Add(boat);
                        break;
                    }
                }
            }

            if (!canDock)
            {
                for (int i = secondaryDocks.Count - 1; i >= 0; i--)
                {
                    if (!secondaryDocks[i].IsOccupied && i - boat.Size - 1 >= 0)
                    {
                        canDock = true;

                        for (int j = boat.Size - 1; j >= 0; j--)
                            if (secondaryDocks[i - j].IsOccupied)
                                canDock = false;

                        if (canDock)
                        {
                            for (int k = boat.Size - 1; k >= 0; k--)
                            {
                                secondaryDocks[i - k].IsOccupied = true;
                                boat.DockedTo.Add(secondaryDocks[i - k].DockNumber);
                                DockedBoats.Add(boat);
                            }
                            break;
                        }
                    }
                }
            }
        }

        private List<Boat> GenerateIncomingBoats(int numberOfIncomingBoats)
        {
            List<Boat> incomingBoats = new List<Boat>(numberOfIncomingBoats);

            int rnd;

            for (int i = 0; i < numberOfIncomingBoats; i++)
            {
                rnd = Utils.RandomNumberInRange(1, 5);
                switch (rnd)
                {
                    case 1:
                        incomingBoats.Add(new RowingBoat());
                        break;
                    case 2:
                        incomingBoats.Add(new MotorBoat());
                        break;
                    case 3:
                        incomingBoats.Add(new SailingBoat());
                        break;
                    case 4:
                        incomingBoats.Add(new Catamaran());
                        break;
                    case 5:
                        incomingBoats.Add(new CargoShip());
                        break;
                }
            }

            return incomingBoats;
        }





    }
}
