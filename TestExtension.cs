using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using bimeh_back.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using bimeh_back.Components.Response;
using Xunit;
using Xunit.Abstractions;

namespace bimeh_back.Components.Extensions
{
    public abstract class TestExtension
    {
        protected IHost Host { get; set; }
        protected IServiceProvider ServiceProvider { get; set; }
        protected readonly ITestOutputHelper Output;

        protected TestExtension(ITestOutputHelper output)
        {
            Output = output;
            CreateHost();
            CreateServiceProvider();
        }

        public static dynamic ToExpandoObject(object o)
            => JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(o));

        public static List<dynamic> ToExpandoList(object o)
            => JsonConvert.DeserializeObject<List<ExpandoObject>>(JsonConvert.SerializeObject(o))
                .Select(x => (dynamic) x)
                .ToList();

        protected object RequestController(Type controller)
        {
            var constructor = GetControllerConstructorInfo(controller);
            if (constructor == null) {
                return null;
            }

            var parameters = CreateConstructorParameters(constructor);

            return Activator.CreateInstance(controller, parameters);
        }

        protected AppDbContext RequestDbContext()
        {
            return ServiceProvider.GetService<AppDbContext>();
        }

        protected T RequestDbContext<T>()
        {
            return ServiceProvider.GetService<T>();
        }

        private object[] CreateConstructorParameters(ConstructorInfo constructor)
        {
            return constructor.GetParameters()
                .Select(parameter => ServiceProvider.GetService(parameter.ParameterType))
                .ToArray();
        }

        private ConstructorInfo GetControllerConstructorInfo(Type controller)
        {
            if (controller.GetConstructors().Length < 1) {
                return null;
            }

            return controller.GetConstructors()[0];
        }

        private void CreateServiceProvider()
        {
            using var scope = Host.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            ServiceProvider = scopeFactory.CreateScope().ServiceProvider;
        }

        private void CreateHost()
        {
            var hostBuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());

            Host = hostBuilder.Build();
        }
    }
}