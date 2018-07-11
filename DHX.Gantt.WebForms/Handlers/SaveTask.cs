using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DHX.Gantt.WebForms.Models;
using System.Data.Entity;

namespace DHX.Gantt.WebForms
{
    /// <summary>
    /// Summary description for save_task
    /// </summary>
    public class SaveTaskHandler : IHttpHandler
    {

        private GanttContext _db = new GanttContext();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            try
            {
                switch (context.Request.HttpMethod)
                {
                    case "POST":
                        this._CreateTask(_db, context);
                        break;
                    case "PUT":
                        this._UpdateTask(_db, context);
                        break;

                    case "DELETE":
                        this._DeleteTask(_db, context);
                        break;
                    default:
                        context.Response.StatusCode = (int)System.Net.HttpStatusCode.MethodNotAllowed;
                        _Response(new { action = "error" }, context);
                        break;
                }
            }
            catch (Exception e)
            {
                _Response(new { action = "error" }, context);
            }
        }

        private void _Response(object res, HttpContext context)
        {
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            context.Response.Write(serializer.Serialize(res));
        }

        private void _CreateTask(GanttContext db, HttpContext context)
        {
            var form = context.Request.Params;
            var apiTask = new TaskDto
            {
                text = form["text"],
                start_date = form["start_date"],
                duration = int.Parse(form["duration"]),
                progress = !string.IsNullOrEmpty(form["progress"]) ? decimal.Parse(form["progress"]) : 0,
                parent = !string.IsNullOrEmpty(form["progress"]) ? int.Parse(form["parent"]) : 0,
                type = form["type"]
            };

            var newTask = (Task)apiTask;

            newTask.SortOrder = _db.Tasks.Max(t => t.SortOrder) + 1;
            db.Tasks.Add(newTask);
            db.SaveChanges();

            _Response(new
            {
                tid = newTask.Id,
                action = "inserted"
            }, context);
        }
        private void _UpdateTask(GanttContext db, HttpContext context)
        {
            var form = context.Request.Params;
            var id = int.Parse((string)context.Request.RequestContext.RouteData.Values["id"]);
            var apiTask = new TaskDto
            {
                id = id,
                text = form["text"],
                start_date = form["start_date"],
                duration = int.Parse(form["duration"]),
                progress = !string.IsNullOrEmpty(form["progress"]) ? decimal.Parse(form["progress"]) : 0,
                parent = !string.IsNullOrEmpty(form["progress"]) ? int.Parse(form["parent"]) : 0,
                type = form["type"],
                target = form["target"]
            };

            var updatedTask = (Task)apiTask;

            var dbTask = db.Tasks.Find(updatedTask.Id);
            dbTask.Text = updatedTask.Text;
            dbTask.StartDate = updatedTask.StartDate;
            dbTask.Duration = updatedTask.Duration;
            dbTask.ParentId = updatedTask.ParentId;
            dbTask.Progress = updatedTask.Progress;
            dbTask.Type = updatedTask.Type;

            if (!string.IsNullOrEmpty(apiTask.target))
            {
                // reordering occurred
                this._UpdateOrders(db, dbTask, apiTask.target);
            }

            db.SaveChanges();

            _Response(new
            {
                action = "updated"
            }, context);
        }
        private void _DeleteTask(GanttContext db, HttpContext context)
        {
            var id = int.Parse((string)context.Request.RequestContext.RouteData.Values["id"]);
            var task = db.Tasks.Find(id);
            if (task != null)
            {
                db.Tasks.Remove(task);
                db.SaveChanges();
            }
            _Response(new
            {
                action = "deleted"
            }, context);
        }

        private void _UpdateOrders(GanttContext db, Task updatedTask, string orderTarget)
        {
            int adjacentTaskId;
            var nextSibling = false;

            var targetId = orderTarget;

            // adjacent task id is sent either as '{id}' or as 'next:{id}' depending 
            // on whether it's the next or the previous sibling
            if (targetId.StartsWith("next:"))
            {
                targetId = targetId.Replace("next:", "");
                nextSibling = true;
            }

            if (!int.TryParse(targetId, out adjacentTaskId))
            {
                return;
            }

            var adjacentTask = db.Tasks.Find(adjacentTaskId);
            var startOrder = adjacentTask.SortOrder;

            if (nextSibling)
                startOrder++;

            updatedTask.SortOrder = startOrder;

            var updateOrders = db.Tasks
             .Where(t => t.Id != updatedTask.Id)
             .Where(t => t.SortOrder >= startOrder)
             .OrderBy(t => t.SortOrder);

            var taskList = updateOrders.ToList();

            taskList.ForEach(t => t.SortOrder++);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}