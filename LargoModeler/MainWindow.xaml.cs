using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace LargoModeler
{

    /// <summary>
    /// Main Window.
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Generate(object sender, RoutedEventArgs e)
        {
            var numberOfBars = 8;
            for (int barNumber = 1; barNumber <= numberOfBars; barNumber++) {
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var staffBars = new List<StaffBar>(); //// ObservableCollection
            var bar = new StaffBar(1);
            bar.staffZones.Add(new StaffZone("1"));
            bar.staffZones.Add(new StaffZone("2"));
            staffBars.Add(bar);

            bar = new StaffBar(2);
            bar.staffZones.Add(new StaffZone("1"));
            bar.staffZones.Add(new StaffZone("2"));
            staffBars.Add(bar);

            bar = new StaffBar(3);
            bar.staffZones.Add(new StaffZone("1"));
            bar.staffZones.Add(new StaffZone("2"));
            staffBars.Add(bar);

            bar = new StaffBar(4);
            bar.staffZones.Add(new StaffZone("1"));
            bar.staffZones.Add(new StaffZone("2"));
            bar.staffZones.Add(new StaffZone("3"));
            staffBars.Add(bar);

            /// this.dataGrid1.DataContext = Records;
            this.dataGridBars.ItemsSource = staffBars;

            //// var staffZones = new List<StaffZone>(); //// ObservableCollection
            //// this.dataGrid1.DataContext = Records;
            //// this.dataGridZones.ItemsSource = staffZones;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
        }

        private void Play(object sender, RoutedEventArgs e)
        {
        }

        private void NumberOfBarsChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void dataGridBars_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var staffBar = this.dataGridBars.SelectedItem as StaffBar;
            if (staffBar == null) {
                return;
            }

            this.dataGridZones.ItemsSource = staffBar.staffZones;
        }
    }
}
