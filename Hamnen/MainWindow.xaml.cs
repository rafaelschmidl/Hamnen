using System;
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

            if (File.Exists(@"data/boat_info.json"))
                Boat_Info.Text = GetBoatInfoFromJsonFile(@"data/boat_info.json");

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
            Boat_Info.Text = "";
            SetBoatInfoToJsonFile(Boat_Info.Text, @"data/boat_info.json");

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
                System.Threading.Thread.Sleep(500);
            }
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            harbor.NextDay(5);

            ClearBoats();
            ShowBoats();


            Boat_Info.Text = "";

            harbor.DockedBoats.ForEach(b => Boat_Info.Text += b.Id + " " + b.DaysBeforeDeparture + "\n");

            Boat_Info.Text += harbor.DockedBoats.Count;


            SetBoatInfoToJsonFile(Boat_Info.Text, @"data/boat_info.json");
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

            if (boat is RowingBoat == false)
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

        private void SetBoatInfoToJsonFile(string boatInfo, string filePath)
        {
            using StreamWriter sw = new StreamWriter(filePath, false);
            sw.Write(JsonSerializer.Serialize(boatInfo));
        }

        private string GetBoatInfoFromJsonFile(string filePath)
        {
            using StreamReader sr = File.OpenText(filePath);
            return JsonSerializer.Deserialize<string>(sr.ReadToEnd());
        }
    }
}
