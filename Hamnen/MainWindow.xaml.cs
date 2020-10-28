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

namespace HarborSimuation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool runLoop = false;

        BackgroundWorker backgroundWorker = new BackgroundWorker();

        Harbor harbor = new Harbor();

        public MainWindow()
        {
            InitializeComponent();

          

            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.WorkerReportsProgress = true;


        }



        private void ShowBoat(StackPanel stackPanelDocks, Dock d)
        {
            Brush brush;
            switch (d.OccupiedBy.GetType().Name)
            {
                case "RowingBoat":
                    brush = Brushes.Yellow;
                    break;
                case "MotorBoat":
                    brush = Brushes.DarkSeaGreen;
                    break;
                case "SailingBoat":
                    brush = Brushes.Azure;
                    break;
                case "Catamaran":
                    brush = Brushes.Goldenrod;
                    break;
                case "CargoShip":
                    brush = Brushes.Black;
                    break;
                default:
                    brush = Brushes.Green;
                    break;
            }


            Rectangle r1 = stackPanelDocks.FindName("Dock_" + d.DockNumber * 2) as Rectangle;
            r1.Fill = brush;

            if (!d.HasPlaceForeSecondRowingBoat)
            {
                Rectangle r2 = stackPanelDocks.FindName("Dock_" + (d.DockNumber * 2 - 1)) as Rectangle;
                r2.Fill = brush;
            }
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (!runLoop)
                runLoop = true;
            else if (runLoop)
                runLoop = false;

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
            Boat_Info.Text += "new day \n";

            harbor.NextDay(1);

            foreach (Dock d in harbor.Docks.DocksLeft.Where(d => d.IsOccupied))
            {
                ShowBoat(Docks_left, d);
                
            }
            foreach (Dock d in harbor.Docks.DocksRight.Where(d => d.IsOccupied))
            {
                ShowBoat(Docks_right, d);


            }

        }
    }
}
