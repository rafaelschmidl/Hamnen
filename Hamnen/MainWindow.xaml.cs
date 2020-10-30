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
        private Harbor harbor = new Harbor();
        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();

            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;

            PaintOrEraseBoats();



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

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            harbor.ResetDocks();
            PaintOrEraseBoats();
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
            PaintOrEraseBoats();
        }

        private void PaintOrEraseBoats()
        {
            foreach (Dock d in harbor.DocksLeft)
            {
                PaintOrEraseBoat(Docks_left, d);
            }
            foreach (Dock d in harbor.DocksRight)
            {
                PaintOrEraseBoat(Docks_right, d);
            }
        }

        private void PaintOrEraseBoat(StackPanel stackPanelDocks, Dock d)
        {
            Brush brush;
            if (!d.IsOccupied())
            {
                brush = null;
            }
            else
            {
                switch (d.OccupiedBy.Id.Substring(0, 2))
                {
                    case "RB":
                        brush = Brushes.Yellow;
                        break;
                    case "MB":
                        brush = Brushes.DarkSeaGreen;
                        break;
                    case "SB":
                        brush = Brushes.Azure;
                        break;
                    case "CM":
                        brush = Brushes.Goldenrod;
                        break;
                    case "CS":
                        brush = Brushes.Black;
                        break;
                    default:
                        brush = Brushes.Green;
                        break;
                }
            }



            Rectangle r1 = stackPanelDocks.FindName("Dock_" + d.IdNumber * 2) as Rectangle;
            r1.Fill = brush;

            if (!d.HasPlaceForSecondRowingBoat())
            {
                Rectangle r2 = stackPanelDocks.FindName("Dock_" + (d.IdNumber * 2 - 1)) as Rectangle;
                r2.Fill = brush;
            }
        }


    }
}
