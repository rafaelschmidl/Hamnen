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

namespace HarborSimuation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Harbor harbor = new Harbor();

            harbor.NextDay(5);


            //harbor.Docks.DocksLeft[0].OccupiedBy = new RowingBoat("RB-L");
            //harbor.Docks.DocksLeft[0].OtherRowingBoat = new RowingBoat("RB-L");
            //harbor.Docks.DocksLeft[1].OccupiedBy = new RowingBoat("RB-L");
            //harbor.Docks.DocksLeft[2].OccupiedBy = new RowingBoat("RB-L");
            //harbor.Docks.DocksLeft[3].OccupiedBy = new SailingBoat("SB-L");

            //harbor.Docks.DocksRight[0].OccupiedBy = new RowingBoat("RB-R");
            //harbor.Docks.DocksRight[31].OccupiedBy = new SailingBoat("SB-R");

            



            // Boat info text

            foreach (Dock d in harbor.Docks.DocksLeft.Where(d => d.IsOccupied))
            {
                ShowBoat(Docks_left, d);
                Boat_Info.Text += d.OccupiedBy.ID + "\n";
            }
            foreach (Dock d in harbor.Docks.DocksRight.Where(d => d.IsOccupied))
            {
                ShowBoat(Docks_right, d);
                Boat_Info.Text += d.OccupiedBy.ID + "\n";
            }


        }

        public void ShowBoat(StackPanel stackPanelDocks, Dock d)
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
    }
}
