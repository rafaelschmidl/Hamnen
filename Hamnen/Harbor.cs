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
        public Dock[] DocksLeft { get; }
        public Dock[] DocksRight { get; }

        public Harbor()
        {
            DocksLeft = new Dock[32];
            DocksRight = new Dock[32];

            if (!File.Exists(@"data/docks_left.json") || !File.Exists(@"data/docks_left.json"))
                ConstructDocks();
            else
            {
                DocksLeft = GetDocksFromJsonFile(@"data/docks_left.json");
                DocksRight = GetDocksFromJsonFile(@"data/docks_right.json");
            }
        }

        public void NextDay(int numberOfIncomingBoats)
        {
            DockIncomingBoats(GenerateIncomingBoats(numberOfIncomingBoats));
            SetDocksToJsonFile(DocksLeft, @"data/docks_left.json");
            SetDocksToJsonFile(DocksRight, @"data/docks_right.json");
        }

        public void ClearDocks()
        {
            ConstructDocks();
            SetDocksToJsonFile(DocksLeft, @"data/docks_left.json");
            SetDocksToJsonFile(DocksRight, @"data/docks_right.json");
        }

        private void ConstructDocks()
        {
            for (int i = 0; i < DocksLeft.Length + DocksRight.Length; i++)
            {
                if (i < DocksLeft.Length)
                {
                    DocksLeft[i] = new Dock(i + 1);
                }
                else
                {
                    DocksRight[i - DocksLeft.Length] = new Dock(i + 1);
                }
            }
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

        private void SetDocksToJsonFile(Dock[] docks, string filePath)
        {
            using StreamWriter sw = new StreamWriter(filePath, false);
            sw.Write(JsonSerializer.Serialize(docks));
        }

        private Dock[] GetDocksFromJsonFile(string filePath)
        {
            using StreamReader sr = File.OpenText(filePath);
            return JsonSerializer.Deserialize<Dock[]>(sr.ReadToEnd());
        }

    }
}
