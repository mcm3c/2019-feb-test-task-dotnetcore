using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TestTask.Models;
using TestTask.Services;
using Microsoft.AspNet.OData;
using System.Threading.Tasks;

namespace TestTask.Controllers {
  public class LocationsController : Controller {
    private JobsService jobsService;
    public LocationsController(JobsService newJobsService) {
      jobsService = newJobsService;
    }


    [EnableQuery]
    public async Task<IEnumerable<Location>> Get() {
      return await jobsService.GetAllLocations();
    }

  }
}
