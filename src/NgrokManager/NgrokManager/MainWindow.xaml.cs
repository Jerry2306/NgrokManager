using NgrokManager.ViewModel;
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

namespace NgrokManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                vm = new MainWindowViewModel();
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Fehler beim ViewModel erstellen: {exc.Message}");
            }
            DataContext = vm;
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Willst du wirklich den Link erneuern?", "Achtung", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                vm.RefreshAddress();
        }

        private void BtnMain_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (vm.MainButtonContent.ToLower() == "stop" && MessageBox.Show("Willst du wirklich stoppen?", "Achtung", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    vm.StartStop();
                else
                    vm.StartStop();
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Fehler beim {vm.MainButtonContent}: {exc.Message}");
            }
        }

        private void BtnRunBatch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vm.StartAPI();
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Konnte Batch nicht ausführen: {exc.Message}");
            }
        }

        private void BtnStopProcess_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vm.StopAPI();
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Konnte API nicht stoppen: {exc.Message}");
            }
        }
    }
}
