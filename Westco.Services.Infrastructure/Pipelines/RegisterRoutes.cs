using System.Web.Mvc;
using System.Web.Routing;
using Sitecore.Pipelines;

namespace Westco.Services.Infrastructure.Pipelines
{
    public class RegisterRoutes
    {
        public void Process(PipelineArgs args)
        {
            RouteTable.Routes.MapRoute("ChapAuthentication", "sitecore/api/ssc/chapauth/{action}", new
            {
                controller = "ChapAuthentication"
            }, new string[1]
            {
                "Westco.Services.Infrastructure.Mvc"
            });
        }
    }
}