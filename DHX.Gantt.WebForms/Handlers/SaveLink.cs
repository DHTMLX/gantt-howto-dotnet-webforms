using System;
using System.Web;

using DHX.Gantt.WebForms.Models;
using System.Data.Entity;

namespace DHX.Gantt.WebForms
{
    /// <summary>
    /// Summary description for save_link
    /// </summary>
    public class SaveLinkHandler : IHttpHandler
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
                        this._CreateLink(_db, context);
                        break;
                    case "PUT":
                        this._UpdateLink(_db, context);
                        break;

                    case "DELETE":
                        this._DeleteLink(_db, context);
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

        private void _CreateLink(GanttContext db, HttpContext context)
        {
            var form = context.Request.Params;
            var linkDto = new LinkDto
            {
                type = form["type"],
                source = int.Parse(form["source"]),
                target = int.Parse(form["target"]),
            };

            var newLink = (Link)linkDto;
            db.Links.Add(newLink);
            db.SaveChanges();

            _Response(new
            {
                tid = newLink.Id,
                action = "inserted"
            }, context);
        }
        private void _UpdateLink(GanttContext db, HttpContext context)
        {
            var form = context.Request.Params;
            var id = int.Parse((string)context.Request.RequestContext.RouteData.Values["id"]);
            var linkDto = new LinkDto
            {
                id = id,
                type = form["type"],
                source = int.Parse(form["source"]),
                target = int.Parse(form["target"]),
            };

            var clientLink = (Link)linkDto;

            db.Entry(clientLink).State = EntityState.Modified;
            db.SaveChanges();

            _Response(new
            {
                action = "updated"
            }, context);
        }
        private void _DeleteLink(GanttContext db, HttpContext context)
        {
            var id = int.Parse((string)context.Request.RequestContext.RouteData.Values["id"]);
            var link = db.Links.Find(id);
            if (link != null)
            {
                db.Links.Remove(link);
                db.SaveChanges();
            }
            _Response(new
            {
                action = "deleted"
            }, context);
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