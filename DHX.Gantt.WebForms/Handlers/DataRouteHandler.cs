using System.Web;

namespace DHX.Gantt.WebForms
{
    public class GenericGanttHandler<T>: System.Web.Routing.IRouteHandler
        where T : IHttpHandler, new()
    {
        public IHttpHandler GetHttpHandler(System.Web.Routing.RequestContext requestContext)
        {
           
            return new T();
        }
    }
}