using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using nib.Models;

namespace nib.Services {
  public class JobsService {
    private const string LOCATIONS_CACHE_KEY = "LOCATIONS";
    private const string JOBS_CACHE_KEY = "JOBS";
    private const int CACHE_EXPIRATION_TIME_SECONDS = 300;
    private IHttpClientFactory clientFactory;
    private IMemoryCache memoryCache;
    private IConfiguration configuration;
    private ILogger logger;

    public JobsService(IConfiguration configuration, IHttpClientFactory clientFactory, IMemoryCache memoryCache, ILogger<JobsService> logger) {
      this.clientFactory = clientFactory;
      this.memoryCache = memoryCache;
      this.configuration = configuration;
      this.logger = logger;
    }

    public IEnumerable<Job> GetAllJobs() {
      List<Job> jobs;
      if (memoryCache.TryGetValue(JOBS_CACHE_KEY, out jobs)) {
        return jobs;
      }
      logger.LogInformation("Populating Jobs from the local file");
      var serializedJobs = File.ReadAllText(configuration["JobsJsonPath"]);
      jobs = JsonConvert.DeserializeObject<List<Job>>(serializedJobs);
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromSeconds(CACHE_EXPIRATION_TIME_SECONDS));
      memoryCache.Set(LOCATIONS_CACHE_KEY, jobs, cacheEntryOptions);

      return jobs;
    }

    public async Task<IEnumerable<Location>> GetAllLocations() {
      List<Location> locations;
      if (memoryCache.TryGetValue(LOCATIONS_CACHE_KEY, out locations)) {
        return locations;
      }

      logger.LogInformation("Requesting locations from the remote API");
      var client = clientFactory.CreateClient();
      var response = await client.GetStringAsync(configuration["LocationApiEndpoint"]);
      locations = JsonConvert.DeserializeObject<List<Location>>(response);
      var jobs = GetAllJobs();
      Dictionary<int, Location> locationsDictionary = new Dictionary<int, Location>();
      foreach (var location in locations) {
        locationsDictionary.Add(location.ID, location);
      }

      foreach (var job in jobs) {
        locationsDictionary[job.LocationID].Jobs.Add(job);
      }
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromSeconds(CACHE_EXPIRATION_TIME_SECONDS));
      memoryCache.Set(LOCATIONS_CACHE_KEY, locations, cacheEntryOptions);


      return locations;
    }
  }
}
