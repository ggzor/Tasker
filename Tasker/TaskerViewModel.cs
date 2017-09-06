using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Foundation;
using System.Windows.Input;

using MoreLinq;
using Newtonsoft.Json;

namespace Tasker
{
    public class TaskerViewModel : ValidatableModel
    {

        public static readonly DirectoryInfo TasksPath = new DirectoryInfo("");

        public static string CurrentFilePath => Path.Combine(TasksPath.FullName, DateTime.Today.ToString("yyyy-MM-dd") + ".json");

        private ObservableCollection<Core.Task> todayTasks = new ObservableCollection<Core.Task>();

        public ObservableCollection<Core.Task> TodayTasks
        {
            get { return todayTasks; }
            set { todayTasks = value; }
        }

        public ICommand GetTodayTasksCommand { get; }

        public ICommand SaveTasksCommand { get; }

        public TaskerViewModel()
        {
            GetTodayTasksCommand = new DelegateCommand(async () => await GetTodayTasks());
            SaveTasksCommand = new DelegateCommand(async () => await SaveTasks());
        }

        public async System.Threading.Tasks.Task GetTodayTasks()
        {
            Task<string> ReadFile(string path) => System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    return File.ReadAllText(path);
                }
                catch (IOException)
                {
                    return "";
                }

            });

            var text = await ReadFile(CurrentFilePath);

            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    var tasks = JsonConvert.DeserializeObject<Core.Task[]>(text);

                    tasks.ForEach(TodayTasks.Add);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error has ocurred while deserializing the tasks: " + ex.Message);
                }
            }
        }


        public async System.Threading.Tasks.Task SaveTasks()
        {
            async System.Threading.Tasks.Task SaveToFile(string path, string value)
            {
                try
                {
                    using (var file = File.Open(path, FileMode.Create))
                    using (var stream = new StreamWriter(file))
                        await stream.WriteAsync(value);
                }
                catch (IOException ex)
                {
                    Console.WriteLine("An error has ocurred while writing the file: " + ex.Message);
                }
            }

            try
            {
                var text = JsonConvert.SerializeObject(TodayTasks.ToArray(), Formatting.Indented);

                await SaveToFile(CurrentFilePath, text);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error has ocurred while serializing the tasks: " + ex.Message);
            }
        }
    }
}
