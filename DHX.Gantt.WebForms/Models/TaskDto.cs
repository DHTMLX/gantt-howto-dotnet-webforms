using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DHX.Gantt.WebForms.Models
{
    public class TaskDto
    {
        public int id { get; set; }
        public string text { get; set; }
        public string start_date { get; set; }
        public int duration { get; set; }
        public decimal progress { get; set; }
        public int? parent { get; set; }
        public string type { get; set; }
        public bool open { get { return true; } set { } }
        public string target { get; set; }

        //[JsonProperty(PropertyName = "$has_child")]
       // public bool has_children { get { return true; } set { } }

        public static explicit operator TaskDto(Task task)
        {
            return new TaskDto
            {
                id = task.Id,
                text = HttpUtility.HtmlEncode(task.Text),
                start_date = task.StartDate.ToString("yyyy-MM-dd HH:mm"),
                duration = task.Duration,
                parent = task.ParentId,
                type = task.Type,
                progress = task.Progress
            };
        }

        public static explicit operator Task(TaskDto task)
        {

            return new Task
            {
                Id = task.id,
                Text = task.text,
                StartDate = DateTime.Parse(task.start_date, System.Globalization.CultureInfo.InvariantCulture),
                Duration = task.duration,
                ParentId = task.parent,
                Type = task.type,
                Progress = task.progress
            };
        }
    }
}