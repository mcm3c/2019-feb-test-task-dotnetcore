using System;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using nib.Models;
using nib.Services;

namespace nib {
  public class Startup {
    public void ConfigureServices(IServiceCollection services) {
      services.AddOData();
      services.AddMvc(option => option.EnableEndpointRouting = false);
      services.AddControllers();
      services.AddSingleton<JobsService>();
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
