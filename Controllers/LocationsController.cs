using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using nib.Models;
using nib.Services;
using Microsoft.AspNet.OData;
using System.Threading.Tasks;

namespace nib.Controllers {
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
