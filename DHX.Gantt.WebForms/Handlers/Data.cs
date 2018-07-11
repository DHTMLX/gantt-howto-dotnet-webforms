using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DHX.Gantt.WebForms.Models;

namespace DHX.Gantt.WebForms
{
    /// <summary>
    /// Summary description for data
    /// </summary>
    public class DataHandler : IHttpHandler
    {
        private GanttContext _db = new GanttContext();
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();


            context.Response.Write(serializer.Serialize(new
            {
                data = _db
                    .Tasks
                    .OrderBy(t => t.SortOrder)
                    .ToList()
                    .Select(t => (TaskDto)t),
                links = _db
                    .Links
                    .ToList()
                    .Select(l => (LinkDto)l)
            })
            );
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