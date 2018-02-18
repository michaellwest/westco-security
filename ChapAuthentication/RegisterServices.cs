using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Services.Infrastructure.Web.Http.Security;

namespace Westco.Services.Infrastructure
{
    internal class RegisterServices : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITokenProvider, ChapTokenProvider>();
        }
    }
}