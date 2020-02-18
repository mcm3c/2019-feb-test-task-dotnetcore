using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using nib.Models;
using nib.Services;
using Microsoft.AspNet.OData;
using System.Threading.Tasks;

namespace nib.Controllers {
  public class JobsController : Controller {
    private JobsService jobsService;
    public JobsController(JobsService newJobsService) {
      jobsService = newJobsService;
    }


    [EnableQuery]
    public IEnumerable<Job> Get() {
      return jobsService.GetAllJobs();
    }

  }
}
