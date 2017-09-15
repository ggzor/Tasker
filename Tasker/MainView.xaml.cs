using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace Tasker
{
    /// <summary>
    /// Lógica de interacción para MainView.xaml
    /// </summary>
    public partial class MainView : MetroWindow
    {
        public TaskerViewModel ViewModel { get; }

        public MainView()
        {
            InitializeComponent();

            ViewModel = MainContainer.DataContext as TaskerViewModel;
        }

        private void ColorZone_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount > 1)
                    WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
                else
                    DragMove();
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private async void DialogHost_DialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            bool create = eventArgs.Parameter as bool? ?? false;

            if (create)
            {
                var task = new Core.Task() { Description = TaskDescription.Text };

                ViewModel.TodayTasks.Add(task);

                try
                {
                    await ViewModel.SaveTasks();

                    TaskDescription.Text = "";
                }
                catch { }
            }
        }
    }
}
