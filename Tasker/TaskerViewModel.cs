﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public static readonly string TasksPath = "C:/Tasker";

        public static string CurrentFilePath => Path.Combine(TasksPath, DateTime.Today.ToString("yyyy-MM-dd") + ".json");

        private ObservableCollection<Core.Task> todayTasks = new ObservableCollection<Core.Task>();

        private string statusBarText = "Ready.";

        public ObservableCollection<Core.Task> TodayTasks
        {
            get { return todayTasks; }
            set { SetProperty(ref todayTasks, value); }
        }

        public string StatusBarText
        {
            get { return statusBarText; }
            set { SetProperty(ref statusBarText, value); }
        }


        public ICommand GetTodayTasksCommand { get; }

        public ICommand SaveTasksCommand { get; }
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented
        };

        public TaskerViewModel()
        {
            GetTodayTasksCommand = new DelegateCommand(async () => await GetTodayTasks());
            SaveTasksCommand = new DelegateCommand(async () => await SaveTasks());

            TodayTasks.CollectionChanged += TodayTasksChanged;

            async void LoadTasks() => await GetTodayTasks();

            if (File.Exists(CurrentFilePath))
                LoadTasks();
            else
            {
                StatusBarText = "No tasks for today were found at the current directory.";

                Directory.CreateDirectory(TasksPath);
            }
        }

        private void TodayTasksChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            e.NewItems?.OfType<Core.Task>().ForEach(t => t.PropertyChanged += TaskChanged);
            e.OldItems?.OfType<Core.Task>().ForEach(t => t.PropertyChanged -= TaskChanged);
        }

        private void TaskChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => SaveTasksCommand.Execute(null);

        public async Task GetTodayTasks()
        {
            Task<string> ReadFile(string path) => Task.Run(() =>
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
                    var tasks = JsonConvert.DeserializeObject<List<Core.Task>>(text);

                    todayTasks.Clear();
                    tasks.ForEach(TodayTasks.Add);
                }
                catch (Exception ex)
                {
                    StatusBarText = "An error has ocurred while deserializing the tasks: " + ex.Message;
                }
            }
        }


        public async Task SaveTasks()
        {
            async Task SaveToFile(string path, string value)
            {
                try
                {
                    using (var file = File.Open(path, FileMode.Create))
                    using (var stream = new StreamWriter(file))
                        await stream.WriteAsync(value);
                }
                catch (IOException ex)
                {
                    StatusBarText = "An error has ocurred while writing the file: " + ex.Message;
                }
            }

            try
            {
                var text = JsonConvert.SerializeObject(TodayTasks.ToList(), JsonSettings);

                await SaveToFile(CurrentFilePath, text);
            }
            catch (Exception ex)
            {
                StatusBarText = "An error has ocurred while serializing the tasks: " + ex.Message;
            }
        }
    }
}
