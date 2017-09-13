using System;
using System.Waf.Foundation;

using Newtonsoft.Json;

namespace Tasker.Core
{
    public class Task : ValidatableModel
    {
        private string description;
        private bool completed;

        [JsonProperty]
        public DateTime Date { get; set; } = DateTime.Now;

        [JsonProperty]
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        [JsonProperty]
        public bool Completed
        {
            get { return completed; }
            set { SetProperty(ref completed, value); }
        }

        public override string ToString()
        {
            var completedString = Completed ? "Completed" : "Not done yet";

            return $"{Description} - {completedString}";
        }
    }
}
