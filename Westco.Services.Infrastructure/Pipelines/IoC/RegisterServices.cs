﻿using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Westco.Services.Infrastructure.Security;

namespace Westco.Services.Infrastructure.Pipelines.IoC
{
    internal class RegisterServices : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IChapTokenProvider, ChapTokenProvider>();
        }
    }
}