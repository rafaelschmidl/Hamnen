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
        public List<Dock> DocksLeft { get; set; }
        public List<Dock> DocksRight { get; set; }
        public int RejectedBoats { get; set; }
        public int Day { get; set; }

        public Harbor()
        {
            RejectedBoats = 0;
            Day = 0;

            if (!File.Exists("data/docks_left.json") || !File.Exists("data/docks_right.json") || !File.Exists("data/docked_boats.json"))
            {
                DockedBoats = new List<Boat>();
                ConstructDocks();
                SetDocksToJsonFile(DocksLeft, "data/docks_left.json");
                SetDocksToJsonFile(DocksRight, "data/docks_right.json");
                SetDockedBoatsToJsonFile(DockedBoats, "data/docked_boats.json");
            }
            else
            {
                DocksLeft = GetDocksFromJsonFile("data/docks_left.json");
                DocksRight = GetDocksFromJsonFile("data/docks_right.json");
                DockedBoats = GetDockedBoatsFromJsonFile("data/docked_boats.json");
            }

            if (File.Exists("data/harbor_info.json"))
            {
                List<int> harborInfo = GetHarborInfoFromJsonFile("data/harbor_info.json");
                RejectedBoats = harborInfo[0];
                Day = harborInfo[1];
            }
        }

        public void NextDay(int numberOfIncomingBoats)
        {
            DecrementDaysBeforeDeparture();
            DepartBoats();
            DockIncomingBoats(GenerateIncomingBoats(numberOfIncomingBoats));
            Day++;

            SetDocksToJsonFile(DocksLeft, "data/docks_left.json");
            SetDocksToJsonFile(DocksRight, "data/docks_right.json");
            SetDockedBoatsToJsonFile(DockedBoats, "data/docked_boats.json");
            SetHarborInfoToJsonFile(PackHarborInfo(RejectedBoats, Day), "data/harbor_info.json");
        }

        public void Clear()
        {
            ConstructDocks();
            DockedBoats = new List<Boat>();
            RejectedBoats = 0;
            Day = 0;

            SetDocksToJsonFile(DocksLeft, "data/docks_left.json");
            SetDocksToJsonFile(DocksRight, "data/docks_right.json");
            SetDockedBoatsToJsonFile(DockedBoats, "data/docked_boats.json");
            SetHarborInfoToJsonFile(PackHarborInfo(RejectedBoats, Day), "data/harbor_info.json");

        }

        private void ConstructDocks()
        {
            DocksLeft = new List<Dock>(32);
            DocksRight = new List<Dock>(32);

            for (int i = 0; i < 32; i++)
            {
                DocksLeft.Add(new Dock(i + 1));
                DocksRight.Add(new Dock(i + 32 + 1));
            }
        }

        private List<int> PackHarborInfo(int rejectedBoats, int day)
        {
            List<int> harborInfo = new List<int>(2);
            harborInfo.Add(rejectedBoats);
            harborInfo.Add(day);
            return harborInfo;
        }

        private void DecrementDaysBeforeDeparture()
        {
            DockedBoats.ForEach(boat => boat.DaysBeforeDeparture--);
        }

        private void DepartBoats()
        {
            List<Boat> departingBoats = DockedBoats.Where(boat => boat.DaysBeforeDeparture <= 0).ToList();

            departingBoats.ForEach(boat => {
                boat.DockedTo.ForEach(dockNumber => {

                    Dock dock;

                    if (dockNumber <= DocksLeft.Count)
                        dock = DocksLeft[dockNumber - 1];
                    else
                        dock = DocksRight[dockNumber - DocksLeft.Count - 1];

                    if (boat.Id.Substring(0, 2) == "RB" && !dock.HasPlaceForAnotherRowingBoat)
                        dock.HasPlaceForAnotherRowingBoat = true;
                    else
                    {
                        dock.HasPlaceForAnotherRowingBoat = true;
                        dock.IsOccupied = false;
                    }
                });
            });
            DockedBoats.RemoveAll(b => b.DaysBeforeDeparture <= 0);
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
                            if (boat is RowingBoat == false)
                                primaryDocks[i + k].HasPlaceForAnotherRowingBoat = false;
                            boat.DockedTo.Add(primaryDocks[i + k].DockNumber);
                        }
                        DockedBoats.Add(boat);
                        break;
                    }
                }
                else if (boat is RowingBoat && primaryDocks[i].HasPlaceForAnotherRowingBoat)
                {
                    canDock = true;
                    primaryDocks[i].HasPlaceForAnotherRowingBoat = false;
                    boat.DockedTo.Add(primaryDocks[i].DockNumber);
                    DockedBoats.Add(boat);
                    break;
                }
            }

            if (!canDock)
            {
                for (int i = secondaryDocks.Count - 1; i >= 0; i--)
                {
                    if (!secondaryDocks[i].IsOccupied && i - boat.Size - 1 > 0)
                    {
                        canDock = true;

                        for (int j = boat.Size - 1; j > 0; j--)
                            if (secondaryDocks[i - j].IsOccupied)
                                canDock = false;

                        if (canDock)
                        {
                            for (int k = boat.Size - 1; k >= 0; k--)
                            {
                                secondaryDocks[i - k].IsOccupied = true;
                                if (boat is RowingBoat == false)
                                    secondaryDocks[i - k].HasPlaceForAnotherRowingBoat = false;
                                boat.DockedTo.Add(secondaryDocks[i - k].DockNumber);
                            }
                            DockedBoats.Add(boat);
                            break;
                        }
                    }
                    else if (boat is RowingBoat && secondaryDocks[i].HasPlaceForAnotherRowingBoat)
                    {
                        canDock = true;
                        secondaryDocks[i].HasPlaceForAnotherRowingBoat = false;
                        boat.DockedTo.Add(secondaryDocks[i].DockNumber);
                        DockedBoats.Add(boat);
                        break;
                    }
                }

                if (!canDock) RejectedBoats++;
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

        private void SetDocksToJsonFile(List<Dock> docks, string filePath)
        {
            using StreamWriter sw = new StreamWriter(filePath, false);
            sw.Write(JsonSerializer.Serialize(docks));
        }

        private List<Dock> GetDocksFromJsonFile(string filePath)
        {
            using StreamReader sr = File.OpenText(filePath);
            return JsonSerializer.Deserialize<List<Dock>>(sr.ReadToEnd());
        }

        private void SetDockedBoatsToJsonFile(List<Boat> boats, string filePath)
        {

            using StreamWriter sw = new StreamWriter(filePath, false);
            sw.Write(JsonSerializer.Serialize(boats));
        }

        private List<Boat> GetDockedBoatsFromJsonFile(string filePath)
        {
            using StreamReader sr = File.OpenText(filePath);
            return JsonSerializer.Deserialize<List<Boat>>(sr.ReadToEnd());
        }

        private void SetHarborInfoToJsonFile(List<int> harborInfo, string filePath)
        {

            using StreamWriter sw = new StreamWriter(filePath, false);
            sw.Write(JsonSerializer.Serialize(harborInfo));
        }

        private List<int> GetHarborInfoFromJsonFile(string filePath)
        {
            using StreamReader sr = File.OpenText(filePath);
            return JsonSerializer.Deserialize<List<int>>(sr.ReadToEnd());
        }
    }
}
