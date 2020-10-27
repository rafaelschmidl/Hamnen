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
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;


        }



        private void ShowBoat(StackPanel stackPanelDocks, Dock d)
        {
            // TODO: cases for boate types

            Rectangle r1 = stackPanelDocks.FindName("Dock_" + d.DockNumber * 2) as Rectangle;
            r1.Fill = Brushes.Black;

            if (!d.HasPlaceForeSecondRowingBoat)
            {
                Rectangle r2 = stackPanelDocks.FindName("Dock_" + (d.DockNumber * 2 - 1)) as Rectangle;
                r2.Fill = Brushes.Black;
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

            while (runLoop)
            {

                //harbor.NextDay(5);

                //foreach (Dock d in harbor.Docks.DocksLeft.Where(d => d.IsOccupied))
                //{
                //    ShowBoat(Docks_left, d);
                //    //Boat_Info.Text += d.OccupiedBy.ID + "\n";
                //}
                //foreach (Dock d in harbor.Docks.DocksRight.Where(d => d.IsOccupied))
                //{
                //    ShowBoat(Docks_right, d);
                //    //Boat_Info.Text += d.OccupiedBy.ID + "\n";
                //}

                backgroundWorker.ReportProgress(1);



                if (backgroundWorker.CancellationPending)
                {
                    break;
                }

                System.Threading.Thread.Sleep(3000);
            }

        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Boat_Info.Text += "new day \n";


        }
    }
}
