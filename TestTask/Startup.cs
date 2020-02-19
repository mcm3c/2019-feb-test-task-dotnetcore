using System.IO.Abstractions;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestTask.Models;
using TestTask.Services;

namespace TestTask {
  public class Startup {
    public void ConfigureServices(IServiceCollection services) {
      services.AddOData();
      services.AddMvc(option => option.EnableEndpointRouting = false);
      services.AddControllers();
      services.AddSingleton<JobsService>();
      services.AddSingleton<IFileSystem, FileSystem>();
      services.AddHttpClient();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      var builder = new ODataConventionModelBuilder();
      builder.EntitySet<Job>("Jobs");
      builder.EntitySet<Location>("Locations");
      app.UseMvc(routerBuilder =>
        {
          routerBuilder.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
          routerBuilder.EnableDependencyInjection();
          routerBuilder.Expand().Select().OrderBy().Filter();
        }
      );
    }
  }
}
