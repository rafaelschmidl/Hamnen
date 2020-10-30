using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Controls;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;

namespace HarborSimuation
{
    public class Harbor
    {
        public Dock[] DocksLeft => docksLeft;
        public Dock[] DocksRight => docksRight;

        private Dock[] docksLeft;
        private Dock[] docksRight;

        public Harbor()
        {
            docksLeft = new Dock[32];
            docksRight = new Dock[32];
            try
            {
                docksLeft = GetDocksFromJsonFile("docks_left");
                docksRight = GetDocksFromJsonFile("docks_right");
            }
            catch
            {
                ConstructDocks();
            }
        }

        public void ResetDocks()
        {
            ConstructDocks();
            SetDocksToJsonFile(docksLeft, "docks_left");
            SetDocksToJsonFile(docksRight, "docks_right");
        }

        public void NextDay(int numberOfIncomingBoats)
        {
            DockIncomingBoats(GenerateIncomingBoats(numberOfIncomingBoats));
            SetDocksToJsonFile(docksLeft, "docks_left");
            SetDocksToJsonFile(docksRight, "docks_right");
        }

        private void DockIncomingBoats(List<Boat> incomingBoats)
        {
            incomingBoats.ForEach(boat => {
                if (boat.Size > 2)
                    DockBoat(boat, DocksLeft, DocksRight);
                else
                    DockBoat(boat, DocksRight, DocksLeft);
            });
        }

        private void DockBoat(Boat boat, Dock[] primaryDocks, Dock[] secondaryDocks)
        {
            bool canDock = false;
            for (int i = 0; i < primaryDocks.Length; i++)
            {
                if (!primaryDocks[i].IsOccupied() && i + boat.Size < primaryDocks.Length)
                {
                    canDock = true;
                    for (int j = 1; j < boat.Size; j++)
                        if (primaryDocks[i + j].IsOccupied())
                            canDock = false;
                    if (canDock)
                    {
                        for (int k = 0; k < boat.Size; k++)
                            primaryDocks[i + k].OccupiedBy = boat;
                        break;
                    }
                }
            }
            if (!canDock)
            {
                for (int i = secondaryDocks.Length - 1; i >= 0; i--)
                {
                    if (!secondaryDocks[i].IsOccupied() && i - boat.Size + 1 >= 0)
                    {
                        canDock = true;
                        for (int j = (int)boat.Size - 1; j >= 0; j--)
                            if (secondaryDocks[i - j].IsOccupied())
                                canDock = false;
                        if (canDock)
                        {
                            for (int k = (int)boat.Size - 1; k >= 0; k--)
                                secondaryDocks[i - k].OccupiedBy = boat;
                            break;
                        }
                    }
                }
            }
        }

        private void ConstructDocks()
        {
            for (int i = 0; i < docksLeft.Length + docksRight.Length; i++)
            {
                if (i < docksLeft.Length)
                {
                    docksLeft[i] = new Dock(i + 1);
                }
                else
                {
                    docksRight[i - docksLeft.Length] = new Dock(i + 1);
                }
            }
        }

        public List<Boat> GenerateIncomingBoats(int numberOfIncomingBoats)
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

        private void SetDocksToJsonFile(Dock[] docks, string fileName)
        {
            using StreamWriter sw = new StreamWriter(@"data/" + fileName + ".json", false);
            sw.Write(JsonSerializer.Serialize(docks));
        }

        private Dock[] GetDocksFromJsonFile(string fileName)
        {
            using StreamReader sr = File.OpenText(@"data/" + fileName + ".json");
            return JsonSerializer.Deserialize<Dock[]>(sr.ReadToEnd());
        }

    }
}
