// Path: Views/MainWindow.xaml.cs

using System.Windows;
using System.Windows.Input;

namespace AxLauncher.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove(); // Перемещение окна
        }
    }
}