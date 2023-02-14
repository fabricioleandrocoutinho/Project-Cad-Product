using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace Project.Cad.Web.Extension
{
    public static class SwaggerExtension
    {
        //
        // Resumo:
        //     Adds camelCase definition to routes
        //
        // Parâmetros:
        //   c:
        //
        //   basePath:
        public static void AddSwaggerCamelCaseDefinition(this SwaggerOptions c, string basePath = "/api/")
        {
            c.PreSerializeFilters.Add(delegate (OpenApiDocument swaggerDoc, HttpRequest httpReq)
            {
                Dictionary<string, OpenApiPathItem> dictionary = swaggerDoc.Paths.ToDictionary((KeyValuePair<string, OpenApiPathItem> item) => FirstCharacterToLower(item.Key, basePath), (KeyValuePair<string, OpenApiPathItem> item) => item.Value);
                swaggerDoc.Paths.Clear();
                foreach (KeyValuePair<string, OpenApiPathItem> item in dictionary)
                {
                    swaggerDoc.Paths.Add(item.Key, item.Value);
                }
            });
        }

        //
        // Resumo:
        //     Adds API servers mapping its origin URL (X-Original-URL) and its correct protocol
        //     (http or https) based on the environment (development or production)
        //
        // Parâmetros:
        //   c:
        //
        //   env:
        public static void AddServers(this SwaggerOptions c, IWebHostEnvironment env)
        {
            c.RouteTemplate = "swagger/{documentName}/swagger.json";
            c.PreSerializeFilters.Add(delegate (OpenApiDocument swaggerDoc, HttpRequest httpReq)
            {
                string text = httpReq.Headers["X-Original-URL"].FirstOrDefault();
                string value = ((text != null) ? text!.Split("/")[1] : null);
                string value2 = (Debugger.IsAttached ? "http" : "https");
                List<OpenApiServer> list = new List<OpenApiServer>();
                OpenApiServer openApiServer = new OpenApiServer();
                DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 3);
                defaultInterpolatedStringHandler.AppendFormatted(value2);
                defaultInterpolatedStringHandler.AppendLiteral("://");
                defaultInterpolatedStringHandler.AppendFormatted(httpReq.Host.Value);
                defaultInterpolatedStringHandler.AppendLiteral("/");
                defaultInterpolatedStringHandler.AppendFormatted(value);
                openApiServer.Url = defaultInterpolatedStringHandler.ToStringAndClear();
                list.Add(openApiServer);
                swaggerDoc.Servers = list;
            });
        }

        internal static string FirstCharacterToLower(string str, string basePath)
        {
            if (string.IsNullOrEmpty(str) || char.IsLower(str, 0))
            {
                return str;
            }

            string[] array = str.Split(basePath);
            return string.Join("", basePath, char.ToLowerInvariant(array[1][0]) + array[1][1..]);
        }
    }
}