using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TestTask.Models;

namespace TestTask.Services {
  public class JobsService {
    public const string LOCATIONS_CACHE_KEY = "LOCATIONS";
    public const string JOBS_CACHE_KEY = "JOBS";
    private const int CACHE_EXPIRATION_TIME_SECONDS = 300;
    private IHttpClientFactory clientFactory;
    private IMemoryCache memoryCache;
    private IConfiguration configuration;
    private IFileSystem fileSystem;

    public JobsService(
      IConfiguration configuration,
      IHttpClientFactory clientFactory,
      IMemoryCache memoryCache,
      IFileSystem fileSystem
    ) {
      this.clientFactory = clientFactory;
      this.memoryCache = memoryCache;
      this.configuration = configuration;
      this.fileSystem = fileSystem;
    }

    /// <summary>
    /// Gets a flat collection of jobs
    /// </summary>
    /// <returns>The all jobs.</returns>
    public IEnumerable<Job> GetAllJobs() {
      List<Job> jobs;
      if (memoryCache.TryGetValue(JOBS_CACHE_KEY, out jobs)) {
        return jobs;
      }
      var serializedJobs = fileSystem.File.ReadAllText(configuration["JobsJsonPath"]);
      jobs = JsonConvert.DeserializeObject<List<Job>>(serializedJobs);
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromSeconds(CACHE_EXPIRATION_TIME_SECONDS));
      memoryCache.Set(LOCATIONS_CACHE_KEY, jobs, cacheEntryOptions);

      return jobs;
    }

    /// <summary>
    /// Gets all locations and enriches with jobs
    /// </summary>
    /// <returns>Locations with jobs.</returns>
    public async Task<IEnumerable<Location>> GetAllLocations() {
      List<Location> locations;
      if (memoryCache.TryGetValue(LOCATIONS_CACHE_KEY, out locations)) {
        return locations;
      }

      var client = clientFactory.CreateClient();
      var response = await client.GetStringAsync(configuration["LocationApiEndpoint"]);
      locations = JsonConvert.DeserializeObject<List<Location>>(response);
      var jobs = GetAllJobs();
      Dictionary<int, Location> locationsDictionary = new Dictionary<int, Location>();
      foreach (var location in locations) {
        locationsDictionary.Add(location.ID, location);
      }

      foreach (var job in jobs) {
        // jobs that don't have a corresponding location, will be skipped
        if (locationsDictionary.ContainsKey(job.LocationID)) {
          locationsDictionary[job.LocationID].Jobs.Add(job);
        }
      }
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromSeconds(CACHE_EXPIRATION_TIME_SECONDS));
      memoryCache.Set(LOCATIONS_CACHE_KEY, locations, cacheEntryOptions);


      return locations;
    }
  }
}
