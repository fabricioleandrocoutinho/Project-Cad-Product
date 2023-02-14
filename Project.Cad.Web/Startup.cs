using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Project.Cad.Data.DB;
using Project.Cad.Data.Entities;
using Project.Cad.Data.Entities.Api.Collections;
using Project.Cad.Data.Interfaces.Repository;
using Project.Cad.Domain.Interface;
using Project.Cad.Domain.Services;
using Project.Cad.Web.Extension;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;

namespace Project.Cad.Web;

[ExcludeFromCodeCoverage]
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {

        services.AddSingleton(_ => MongoDbConfig.GetFromEnvs());
        services.AddSingleton<IMongoConnection, MongoConnection>();
        
        services.AddSingleton<ICrudService<Product>, ProductService>();
        services.AddSingleton<ICrudRepository<Product>, CrudRepository<Product>>();

        services.AddMemoryCache();

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        services.AddSwaggerGen(config =>
        {
            config.DescribeAllParametersInCamelCase();
            config.CustomOperationIds(e => $"{e.HttpMethod}_{e.ActionDescriptor.AttributeRouteInfo.Template.Replace("api/", "").Replace("/", "_").ToUpper()}");
            config.SwaggerDoc("v1", new OpenApiInfo { Title = "Project.Cad.Web", Version = "v1" });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseSwagger(
            c =>
            {
                c.AddSwaggerCamelCaseDefinition();
                c.AddServers(env);
            }
        );

        app.UseSwaggerUI(
            c => { c.SwaggerEndpoint("../swagger/v1/swagger.json", "Project Cad V1"); }
        );

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

}
