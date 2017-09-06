using System;
using System.ComponentModel;

using Newtonsoft.Json;

namespace Tasker.Core
{
    public class Task : INotifyPropertyChanged
    {
        public DateTime Date { get; set; } = DateTime.Now;

        public string Description { get; set; }

        public bool Completed { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Changed() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Representation)));

        public override string ToString()
        {
            var completedString = Completed ? "Completed" : "Not done yet";

            return $"{Description} - {completedString}";
        }

        [JsonIgnore]
        public string Representation => ToString();
    }
}
