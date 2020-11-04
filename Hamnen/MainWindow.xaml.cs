﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using Harbor;
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

        public MainWindow()
        {
            InitializeComponent();

            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;

            if (File.Exists("data/boat_info.json"))
                Boat_Info.Text = GetStringFromJsonFile("data/boat_info.json");
            if (File.Exists("data/summary_info.json"))
                Summary_Info.Text = GetStringFromJsonFile("data/summary_info.json");

            ShowBoats();
        }

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
            SetStringToJsonFile(Boat_Info.Text, "data/boat_info.json");

            Summary_Info.Text = "";
            SetStringToJsonFile(Summary_Info.Text, "data/summary_info.json");

            if (backgroundWorker.IsBusy)
            {
                backgroundWorker.CancelAsync();
                Start_stop_button.Content = "Start";
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (backgroundWorker.CancellationPending)
                    break;
                backgroundWorker.ReportProgress(0);
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            harbor.NextDay(5);
            ClearBoats();
            UpdateBoatInfo();
            UpdateSummaryInfo();
            ShowBoats();
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

            Summary_Info.Text += "\n" +
                " Rowing Boats:\t\t" + rowingBoats + "\t\t" + "Weight Sum:\t\t" + sumWeight + " kg\n" +
                " Motor Boats:\t\t" + motorBoats + "\t\t" + "Average Speed:\t\t" + Math.Round(Utils.KnotsToKmph(averageSpeed), 2) + " km/h\n" +
                " Sailing Boats:\t\t" + sailingBoats + "\t\t" + "Unoccupied Docks:\t" + unoccupiedDocks + "\n" +
                " Catamarans:\t\t" + catamarans + "\t\t" + "Rejected Boats:\t\t" + rejectedBoats + "\n" +
                " Cargo Ships:\t\t" + cargaoShips + "\n";

            SetStringToJsonFile(Summary_Info.Text, "data/summary_info.json");
        }

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

            SetStringToJsonFile(Boat_Info.Text, "data/boat_info.json");
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
                    return Brushes.Black;
                default:
                    return Brushes.Green;
            }
        }

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

        private void BoatsPerDaySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
