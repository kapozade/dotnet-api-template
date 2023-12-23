using Supreme.Api.Controllers;
using Supreme.Infrastructure;
using Supreme.Api.Middlewares;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;
using Microsoft.OpenApi.Models;
using Supreme.Domain.Db;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, serviceProvider, cfg) =>
{
    cfg.ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(serviceProvider);
});

var services = builder.Services;
var configuration = builder.Configuration;
var assemblies = GetProjectAssemblies();
CompositionRoot.Register(services, configuration, assemblies);

services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
    options.SerializerSettings.StringEscapeHandling = StringEscapeHandling.Default;
    options.SerializerSettings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full;
    options.SerializerSettings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
    options.SerializerSettings.TypeNameHandling = TypeNameHandling.None;
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
});

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Supreme API",
        Version = "v1"
    });
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Supreme.Api.xml"));
});

void App()
{
    var app = builder.Build();

    app.UseSwagger();
    app.UseRouting();
    app.MapControllers();
    app.MapHealthChecks("/health");

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Supreme API V1");
        c.RoutePrefix = "";
        c.DocExpansion(DocExpansion.None);
    });

    app.UseGlobalExceptionHandler();
    app.Run();
}

List<Assembly> GetProjectAssemblies()
{
    var startupAssembly = typeof(Program).Assembly;
    var list = new List<Assembly> { startupAssembly };
    list.AddRange(startupAssembly.GetReferencedAssemblies()
        .Where(x => !string.IsNullOrWhiteSpace(x.Name)
                    && x.Name.StartsWith("Supreme"))
        .DistinctBy(x => x.Name).Select(Assembly.Load));

    return list;
}

App();

public partial class Program { }
