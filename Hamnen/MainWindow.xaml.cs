using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using Dock = Harbor.Dock;

namespace HarborSimuation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Harbor harbor = new Harbor();
        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        private int boatsPerDay = 5;
        private int dayIteartionSpeed = 2222;

        public MainWindow()
        {
            InitializeComponent();

            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;

            Boats_per_day_slider_value.Content = boatsPerDay.ToString();
            Day_iteration_speed_slider_value.Content = dayIteartionSpeed.ToString();

            if (File.Exists("boat_info.json"))
                Boat_Info.Text = GetStringFromJsonFile("boat_info.json");
            if (File.Exists("summary_info.json"))
                Summary_Info.Text = GetStringFromJsonFile("summary_info.json");

            ShowBoats();
        }

        // Background worker methods

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Method for looping asynchronously
            while (true)
            {
                if (backgroundWorker.CancellationPending)
                    break;
                backgroundWorker.ReportProgress(0); // Calls 'BackgroundWorker_ProgressChanged' method
                System.Threading.Thread.Sleep(dayIteartionSpeed);
            }
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            harbor.NextDay(boatsPerDay);
            ClearBoats();
            UpdateBoatInfo();
            UpdateSummaryInfo();
            ShowBoats();
        }

        // Control Methods

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync();
                Start_stop_button.Content = "Stop";
            }
            else
            {
                backgroundWorker.CancelAsync();
                Start_stop_button.Content = "Start";
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            harbor.Clear();
            ClearBoats();

            Boat_Info.Text = "";
            SetStringToJsonFile(Boat_Info.Text, "boat_info.json");

            Summary_Info.Text = "";
            SetStringToJsonFile(Summary_Info.Text, "summary_info.json");

            if (backgroundWorker.IsBusy)
            {
                backgroundWorker.CancelAsync();
                Start_stop_button.Content = "Start";
            }
        }

        private void BoatsPerDaySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            Boats_per_day_slider_value.Content = Math.Round(slider.Value).ToString();
            boatsPerDay = (int)slider.Value;
        }

        private void DayIterationSpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            Day_iteration_speed_slider_value.Content = Math.Round(slider.Value).ToString();
            dayIteartionSpeed = (int)slider.Value;
        }

        // Update methods

        private void UpdateBoatInfo()
        {
            Boat_Info.Text = "";

            harbor.DockedBoats.ForEach(b => b.DockedTo.Sort());

            harbor.DockedBoats.OrderBy(b => b.DockedTo.First()).ToList().ForEach(b => {
                int dockNumberMin = b.DockedTo.Min();
                int dockNumberMax = b.DockedTo.Max();

                Boat_Info.Text += " " +
                    dockNumberMin + "-" +
                    dockNumberMax + "\t " +
                    BoatTypeName(b) + "\t" +
                    b.Id + "\t    " +
                    b.Weight + "  \t" +
                    Math.Round(Utils.KnotsToKmph(b.MaxSpeedKnots), 2) + "\t" +
                    UniqueBoatPropertyDescription(b) + " " + b.UniqueProperty + "\n";
            });

            SetStringToJsonFile(Boat_Info.Text, "boat_info.json");
        }

        private void UpdateSummaryInfo()
        {
            Summary_Info.Text = "";

            int rowingBoats = harbor.DockedBoats.Where(b => b.Id.Substring(0, 2) == "RB").ToList().Count();
            int motorBoats = harbor.DockedBoats.Where(b => b.Id.Substring(0, 2) == "MB").ToList().Count();
            int sailingBoats = harbor.DockedBoats.Where(b => b.Id.Substring(0, 2) == "SB").ToList().Count();
            int catamarans = harbor.DockedBoats.Where(b => b.Id.Substring(0, 2) == "CM").ToList().Count();
            int cargaoShips = harbor.DockedBoats.Where(b => b.Id.Substring(0, 2) == "CS").ToList().Count();

            int sumWeight = harbor.DockedBoats.Sum(b => b.Weight);

            double averageSpeed = harbor.DockedBoats.Average(b => b.MaxSpeedKnots);

            int unoccupiedDocks = 0;
            unoccupiedDocks += harbor.DocksLeft.Where(d => !d.IsOccupied).ToList().Count();
            unoccupiedDocks += harbor.DocksRight.Where(d => !d.IsOccupied).ToList().Count();

            int rejectedBoats = harbor.RejectedBoats;

            Summary_Info.Text +=
                " Rowing Boats:\t\t" + rowingBoats + "\t\t" + "Weight Sum:\t\t" + sumWeight + " kg\n" +
                " Motor Boats:\t\t" + motorBoats + "\t\t" + "Average Speed:\t\t" + Math.Round(Utils.KnotsToKmph(averageSpeed), 2) + " km/h\n" +
                " Sailing Boats:\t\t" + sailingBoats + "\t\t" + "Unoccupied Docks:\t" + unoccupiedDocks + "\n" +
                " Catamarans:\t\t" + catamarans + "\t\t" + "Rejected Boats:\t\t" + rejectedBoats + "\n" +
                " Cargo Ships:\t\t" + cargaoShips + "\t\t" + "Day:\t\t\t" + harbor.Day + "\n";

            SetStringToJsonFile(Summary_Info.Text, "summary_info.json");
        }

        private void ShowBoats()
        {
            harbor.DockedBoats.ForEach(boat => {
                boat.DockedTo.ForEach(dockNumber => {
                    FillDockRect(Harbor, dockNumber, boat);
                });
            });
        }

        private void ClearBoats()
        {
            foreach (var rect in Docks_left.Children)
            {
                Rectangle r = rect as Rectangle;
                r.Fill = null;
            }
            foreach (var rect in Docks_right.Children)
            {
                Rectangle r = rect as Rectangle;
                r.Fill = null;
            }
        }

        // Helper methods

        private void FillDockRect(StackPanel stackPanel, int dockNumber, Boat boat)
        {
            Brush brush = BoatColorBrush(boat);

            Rectangle r1 = stackPanel.FindName("Dock_" + dockNumber * 2) as Rectangle;
            r1.Fill = brush;

            Dock dock;
            if (dockNumber < 33)
                dock = harbor.DocksLeft.Find(d => d.DockNumber == dockNumber);
            else
                dock = harbor.DocksRight.Find(d => d.DockNumber == dockNumber);

            if (!dock.HasPlaceForAnotherRowingBoat)//(boat.Id.Substring(0, 2) != "RB")
            {
                Rectangle r2 = stackPanel.FindName("Dock_" + (dockNumber * 2 - 1)) as Rectangle;
                r2.Fill = brush;
            }
        }

        private Brush BoatColorBrush(Boat boat)
        {
            switch (boat.Id.Substring(0, 2))
            {
                case "RB":
                    return Brushes.Yellow;
                case "MB":
                    return Brushes.DarkSeaGreen;
                case "SB":
                    return Brushes.Azure;
                case "CM":
                    return Brushes.Goldenrod;
                case "CS":
                    return Brushes.Red;
                default:
                    return Brushes.Green;
            }
        }

        private string BoatTypeName(Boat boat)
        {
            switch (boat.Id.Substring(0, 2))
            {
                case "RB":
                    return "Rowing Boat";
                case "MB":
                    return "Motot Boat";
                case "SB":
                    return "Sailing Boat";
                case "CM":
                    return "Catamaran";
                case "CS":
                    return "Cargo Ship";
                default:
                    return "";
            }
        }

        private string UniqueBoatPropertyDescription(Boat boat)
        {
            switch (boat.Id.Substring(0, 2))
            {
                case "RB":
                    return "Max Passengers:";
                case "MB":
                    return "Horsepowers:";
                case "SB":
                    return "Length:";
                case "CM":
                    return "Bedds:";
                case "CS":
                    return "Containers:";
                default:
                    return "";
            }
        }

        // Save methods

        private void SetStringToJsonFile(string text, string filePath)
        {
            using StreamWriter sw = new StreamWriter(filePath, false);
            sw.Write(JsonSerializer.Serialize(text));
        }

        private string GetStringFromJsonFile(string filePath)
        {
            using StreamReader sr = File.OpenText(filePath);
            return JsonSerializer.Deserialize<string>(sr.ReadToEnd());
        }
    }
}
