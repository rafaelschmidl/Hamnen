using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Controls;

namespace HarborSimuation
{
    public class Harbor
    {
        public (Dock[] DocksLeft, Dock[] DocksRight) Docks = (new Dock[32], new Dock[32]);



        public Harbor()
        {
            BuildDocks();
        }

        private void BuildDocks()
        {
            for (int i = 0; i < Docks.DocksLeft.Length + Docks.DocksRight.Length; i++)
            {
                if (i < Docks.DocksLeft.Length)
                {
                    Docks.DocksLeft[i] = new Dock(i + 1);
                }
                else
                {
                    Docks.DocksRight[i - Docks.DocksLeft.Length] = new Dock(i + 1);
                }
            }
        }

        public void NextDay(int incomingBoatsPerDay)
        {

            //Docks.DocksLeft.ForEach(d => d.OccupiedBy.DaysBeforeDeparture--);
            //Docks.DocksRight.ForEach(d => d.OccupiedBy.DaysBeforeDeparture--);

            DockIncomingBoats(GenerateIncomingBoats(incomingBoatsPerDay));
        }


        public void DockIncomingBoats(List<Boat> incomingBoats)
        {
            incomingBoats.ForEach(boat => DockBoat(boat));
        }

        private void DockBoat(Boat boat)
        {
            bool canDock = false;

            if (boat.Size > 2)
            {

                for (int i = 0; i < Docks.DocksLeft.Length; i++)
                {
                    if (!Docks.DocksLeft[i].IsOccupied && i + boat.Size < Docks.DocksLeft.Length)
                    {
                        canDock = true;
                        for (int j = 1; j < boat.Size; j++) 
                            if (Docks.DocksLeft[i + j].IsOccupied) 
                                canDock = false;
                        if (canDock)
                        {
                            for (int k = 0; k < boat.Size; k++)
                                Docks.DocksLeft[i + k].OccupiedBy = boat;
                            break;
                        }
                    }
                }
                if (!canDock)
                {
                    for (int i = Docks.DocksRight.Length - 1; i >= 0; i--)
                    {
                        if (!Docks.DocksRight[i].IsOccupied && i - boat.Size + 1 >= 0)
                        {
                            canDock = true;
                            for (int j = (int)boat.Size - 1; j >= 0; j--)
                                if (Docks.DocksRight[i - j].IsOccupied)
                                    canDock = false;
                            if (canDock)
                            {
                                for (int k = (int)boat.Size - 1; k >= 0; k--)
                                    Docks.DocksRight[i - k].OccupiedBy = boat;
                                break;
                            }

                        }
                    }

                }
            }
            else
            {

                for (int i = 0; i < Docks.DocksRight.Length; i++)
                {
                    if (!Docks.DocksRight[i].IsOccupied && i + boat.Size < Docks.DocksRight.Length)
                    {
                        canDock = true;
                        for (int j = 1; j < boat.Size; j++)
                            if (Docks.DocksRight[i + j].IsOccupied)
                                canDock = false;
                        if (canDock)
                        {
                            for (int k = 0; k < boat.Size; k++)
                                Docks.DocksRight[i + k].OccupiedBy = boat;
                            break;
                        }
                    }
                }
                if (!canDock)
                {
                    for (int i = Docks.DocksLeft.Length - 1; i >= 0; i--)
                    {
                        if (!Docks.DocksLeft[i].IsOccupied && i - boat.Size + 1 >= 0)
                        {
                            canDock = true;
                            for (int j = (int)boat.Size - 1; j >= 0; j--)
                                if (Docks.DocksLeft[i - j].IsOccupied)
                                    canDock = false;
                            if (canDock)
                            {
                                for (int k = (int)boat.Size - 1; k >= 0; k--)
                                    Docks.DocksLeft[i - k].OccupiedBy = boat;
                                break;
                            }

                        }
                    }

                }

            }
        }

        public List<Boat> GenerateIncomingBoats(int numberOfIncomingBoats)
        {
            List<Boat> boats = new List<Boat>(numberOfIncomingBoats);

            int rnd;

            for (int i = 0; i < numberOfIncomingBoats; i++)
            {
                rnd = Utils.RandomNumberInRange(1, 5);

                switch (rnd)
                {
                    case 1:
                        boats.Add(new RowingBoat());
                        break;
                    case 2:
                        boats.Add(new MotorBoat());
                        break;
                    case 3:
                        boats.Add(new SailingBoat());
                        break;
                    case 4:
                        boats.Add(new Catamaran());
                        break;
                    case 5:
                        boats.Add(new CargoShip());
                        break;
                }
            }

            return boats;
        }

    }
}
