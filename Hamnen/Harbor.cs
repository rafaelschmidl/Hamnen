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

        bool runLoop = false;



        public Harbor()
        {
            BuildDocks();
        }

        public void Run()
        {

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

            PlaceIncomingBoatsInDocks(GenerateIncomingBoats(incomingBoatsPerDay));
        }

        public void PlaceIncomingBoatsInDocks(List<Boat> incomingBoats)
        {
            int i = 0;
            incomingBoats.ForEach(b =>
            {
                Docks.DocksLeft[i].OccupiedBy = b;
                i++;
            });
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
                        boats.Add(new RowingBoat("RB"));
                        break;
                    case 2:
                        boats.Add(new MotorBoat("MB"));
                        break;
                    case 3:
                        boats.Add(new SailingBoat("SB"));
                        break;
                    case 4:
                        boats.Add(new Catamaran("CM"));
                        break;
                    case 5:
                        boats.Add(new CargoShip("CS"));
                        break;
                }
            }

            return boats;
        }

    }
}
