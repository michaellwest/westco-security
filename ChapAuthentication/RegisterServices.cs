using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Westco.Services.Infrastructure
{
    internal class RegisterServices : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IChapTokenProvider, ChapTokenProvider>();
        }
    }
}