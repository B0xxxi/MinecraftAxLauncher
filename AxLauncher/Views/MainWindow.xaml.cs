// Path: Views/MainWindow.xaml.cs
using System.Windows;
using System.Windows.Input;
using AxLauncher.ViewModels; // Добавьте это пространство имен

namespace AxLauncher.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel; // Изменено на MainViewModel

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel(); // Изменено на MainViewModel
            this.DataContext = viewModel;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove(); // Перемещение окна
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
