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

namespace Tasker
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public TaskerViewModel ViewModel { get; }

        private Func<Core.Task, bool> EqualityComparerFor(string s) => t => t.Description.Equals(s, StringComparison.CurrentCultureIgnoreCase);

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = MainContainer.DataContext as TaskerViewModel;
        }

        private async void AddTask(object sender, RoutedEventArgs e)
        {
            if (TryCreateTask(out var task))
            {
                ViewModel.TodayTasks.Add(task);
                List.SelectedIndex = ViewModel.TodayTasks.Count - 1;

                await SaveChanges();
            }
        }

        private async Task SaveChanges()
        {
            ViewModel.StatusBarText = "Saving...";
            await ViewModel.SaveTasks();
        }

        private bool TryCreateTask(out Core.Task task)
        {
            var description = TaskDescription.Text;
            task = null;

            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("No description has been provided.");
                return false;
            }
            else if (ViewModel.TodayTasks.Any(EqualityComparerFor(description)))
            {
                MessageBox.Show("The task already exists.");
                SelectTaskWithDescription(description);
                return false;
            }
            else
            {
                task = new Core.Task() { Description = description };
                return true;
            }
        }

        private void SelectTaskWithDescription(string description)
        {
            if (TryGetTaskWithDescription(description, out var task))
                List.SelectedItem = task;
        }

        private bool TryGetTaskWithDescription(string description, out Core.Task task)
        {
            task = ViewModel.TodayTasks.SingleOrDefault(EqualityComparerFor(description));

            if (task == null)
                return false;
            else
                return true;
        }

        private async void EditTask(object sender, RoutedEventArgs e)
        {
            var taskToEdit = List.SelectedItem as Core.Task;

            if (TryCreateTask(out var _))
            {
                taskToEdit.Description = TaskDescription.Text;

                await SaveChanges();
            }
        }

        private async void DeleteTask(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this task?. This action cannot be undone.", "Warning",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var task = List.SelectedItem as Core.Task;
                ViewModel.TodayTasks.Remove(task);
                await SaveChanges();
            }
        }

        private async void SwitchTask(object sender, RoutedEventArgs e)
        {
            var task = List.SelectedItem as Core.Task;

            task.Completed = !task.Completed;

            await SaveChanges();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (List.SelectedItems.Count == 1)
                TaskDescription.Text = (List.SelectedItem as Core.Task).Description;
        }
    }
}
