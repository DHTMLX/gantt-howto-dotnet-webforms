using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

using System.Web.Routing;
using System.Data.Entity;
using DHX.Gantt.WebForms.Models;

namespace DHX.Gantt.WebForms
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
            Database.SetInitializer(new GanttInitializer());
        }

        public static void RegisterRoutes(RouteCollection routes)
        { 
            routes.Add(new Route("data", new GenericGanttHandler<DataHandler>())
            {
                Constraints = new RouteValueDictionary {
                    { "httpMethod",  new HttpMethodConstraint(new string[]{ "GET" })}
                }
            });


            var constraints = new RouteValueDictionary {
                { "httpMethod", new HttpMethodConstraint(new string[]{ "PUT", "POST", "DELETE" })}
            };
            var defaults = new RouteValueDictionary { { "id", null } };

            routes.Add(new Route("save/task/{id}", defaults, constraints, new GenericGanttHandler<SaveTaskHandler>()));
            routes.Add(new Route("save/link/{id}", defaults, constraints, new GenericGanttHandler<SaveLinkHandler>()));
        }
    }
}