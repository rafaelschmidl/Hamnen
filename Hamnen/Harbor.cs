using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
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

            if (!File.Exists("docks_left.json") || !File.Exists("docks_right.json") || !File.Exists("docked_boats.json"))
            {
                DockedBoats = new List<Boat>();
                ConstructDocks();
                SetDocksToJsonFile(DocksLeft, "docks_left.json");
                SetDocksToJsonFile(DocksRight, "docks_right.json");
                SetDockedBoatsToJsonFile(DockedBoats, "docked_boats.json");
            }
            else
            {
                DocksLeft = GetDocksFromJsonFile("docks_left.json");
                DocksRight = GetDocksFromJsonFile("docks_right.json");
                DockedBoats = GetDockedBoatsFromJsonFile("docked_boats.json");
            }

            if (File.Exists("harbor_info.json"))
            {
                List<int> harborInfo = GetHarborInfoFromJsonFile("harbor_info.json");
                RejectedBoats = harborInfo[0];
                Day = harborInfo[1];
            }
        }

        // Public update methods

        public void NextDay(int numberOfIncomingBoats)
        {
            DecrementDaysBeforeDeparture();
            DepartBoats();
            DockIncomingBoats(GenerateIncomingBoats(numberOfIncomingBoats));
            Day++;

            SetDocksToJsonFile(DocksLeft, "docks_left.json");
            SetDocksToJsonFile(DocksRight, "docks_right.json");
            SetDockedBoatsToJsonFile(DockedBoats, "docked_boats.json");
            SetHarborInfoToJsonFile(PackHarborInfo(RejectedBoats, Day), "harbor_info.json");
        }

        public void Clear()
        {
            ConstructDocks();
            DockedBoats = new List<Boat>();
            RejectedBoats = 0;
            Day = 0;

            SetDocksToJsonFile(DocksLeft, "docks_left.json");
            SetDocksToJsonFile(DocksRight, "docks_right.json");
            SetDockedBoatsToJsonFile(DockedBoats, "docked_boats.json");
            SetHarborInfoToJsonFile(PackHarborInfo(RejectedBoats, Day), "harbor_info.json");
        }

        // Private update methods

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

        /*
         * Algorithm optimized for harboring as many boats as possible
         * in two rows of docks:
         * 
         * 1 boat length unit = 1 dock
         * 
         * If boat length is bigger than half max boat length
         *      Dock boat (docks A, docks B)
         * Else
         *      Dock boat (docks B, docks A)
         *      
         * - Dock boat (docks A, docks B)
         * For every dock (starting at beginning) in docks A
         *      If the dock and the docks ahead are unoccupied and fit the boat
         *          Dock the boat to those docks
         * 
         * If the boat couldn't dock in docks A
         *      For every dock (backwards starting at end) in docks B
         *          If the dock and the docks behind are unoccupied and fit the boat
         *              Dock the boat to those docks 
         */

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

        // Helper methods

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

        private List<int> PackHarborInfo(int rejectedBoats, int day)
        {
            List<int> harborInfo = new List<int>(2);
            harborInfo.Add(rejectedBoats);
            harborInfo.Add(day);
            return harborInfo;
        }

        // Save methods

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
