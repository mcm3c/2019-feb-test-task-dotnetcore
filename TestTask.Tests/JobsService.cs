using System.Collections.Generic;
using System.IO.Abstractions;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using TestTask.Models;
using TestTask.Services;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using System;

namespace TestTask.UnitTests.Services {
  [TestFixture]
  public class JobsServiceTests {
    [Test]
    public void ReadsJobsFromFile() {
      (
        var jobsService,
        var mockFileSystem,
        var mockConfiguration,
        var mockHttpClientFactory,
        var mockMemoryCache
      ) = CreateJobsService();

      var fakeJob = new Job {
        ID = 9999,
        Name = "Test job"
      };
      var fakeJobs = new List<Job> { fakeJob };
      var serializedFakeJobs = JsonConvert.SerializeObject(fakeJobs);
      var fakeJobsJsonPath = "some_fake_path";
      var appConfigJobsJsonPathKey = "JobsJsonPath";

      mockConfiguration
        .Setup(x => x[It.Is<string>(s => s == appConfigJobsJsonPathKey)])
        .Returns(fakeJobsJsonPath);
      mockFileSystem
        .Setup(x => x.File.ReadAllText(It.Is<string>(s => s == fakeJobsJsonPath)))
        .Returns(serializedFakeJobs);

      var cacheEntry = Mock.Of<ICacheEntry>();
      mockMemoryCache.Setup(x =>
        x.CreateEntry(
          It.IsAny<object>()
        )
      ).Returns(cacheEntry);

      var receivedJobs = jobsService.GetAllJobs().ToList();

      // comparing by value
      Assert.AreEqual(
        JsonConvert.SerializeObject(fakeJobs),
        JsonConvert.SerializeObject(receivedJobs)
      );
    }

    [Test]
    public void ReadsJobsFromCache() {
      (
        var jobsService,
        var mockFileSystem,
        var mockConfiguration,
        var mockHttpClientFactory,
        var mockMemoryCache
      ) = CreateJobsService();

      var fakeJob = new Job {
        ID = 9999,
        Name = "Test job"
      };
      var fakeJobs = new List<Job> { fakeJob };
      // when mocking TryGetValue (in lieu of Get), the out parameter must be declared as an
      // object even if it isn't, if you don't do this then it will match a templated extension
      // method of the same name
      object fakeJobsObject = fakeJobs;

      mockMemoryCache.Setup(x =>
        x.TryGetValue(
          It.Is<string>(s => s == JobsService.JOBS_CACHE_KEY),
          out fakeJobsObject
        )
      ).Returns(true);

      var receivedJobs = jobsService.GetAllJobs().ToList();

      // comparing by value
      Assert.AreEqual(
        JsonConvert.SerializeObject(fakeJobs),
        JsonConvert.SerializeObject(receivedJobs)
      );
    }

    [Test]
    public async Task ReadsLocationsFromApi() {
      (
        var jobsService,
        var mockFileSystem,
        var mockConfiguration,
        var mockHttpClientFactory,
        var mockMemoryCache
      ) = CreateJobsService();

      var fakeJob = new Job {
        ID = 9999,
        Name = "Test job",
        LocationID = 99999
      };
      var fakeJobs = new List<Job> { fakeJob };
      var fakeLocationResponse = @"[{""id"":99999,""name"":""Newcastle"",""state"":""NSW""}]";
      var fakeLocationApiEndpoint = "http://some.fake.url.com";
      var appConfigLocationApiEndpointKey = "LocationApiEndpoint";

      // when mocking TryGetValue (in lieu of Get), the out parameter must be declared as an
      // object even if it isn't, if you don't do this then it will match a templated extension
      // method of the same name
      object fakeJobsObject = fakeJobs;

      mockMemoryCache.Setup(x =>
        x.TryGetValue(
          It.Is<string>(s => s == JobsService.JOBS_CACHE_KEY),
          out fakeJobsObject
        )
      ).Returns(true);
      var cacheEntry = Mock.Of<ICacheEntry>();
      mockMemoryCache.Setup(x =>
        x.CreateEntry(
          It.IsAny<object>()
        )
      ).Returns(cacheEntry);

      mockConfiguration
        .Setup(x => x[It.Is<string>(s => s == appConfigLocationApiEndpointKey)])
        .Returns(fakeLocationApiEndpoint);
      var mockHttp = new MockHttpMessageHandler();
      mockHttp.When(fakeLocationApiEndpoint)
        .Respond("application/json", fakeLocationResponse);
      mockHttpClientFactory
        .Setup(x => x.CreateClient(It.IsAny<string>()))
        .Returns(mockHttp.ToHttpClient());

      var receivedLocations = (await jobsService.GetAllLocations()).ToList();


      Assert.That(receivedLocations.Count(), Is.EqualTo(1));
      Assert.That(receivedLocations[0].ID, Is.EqualTo(99999));
      Assert.That(receivedLocations[0].Name, Is.EqualTo("Newcastle"));
      Assert.That(receivedLocations[0].State, Is.EqualTo("NSW"));
      Assert.That(receivedLocations[0].Jobs.Count(), Is.EqualTo(1));

      // comparing by value
      Assert.AreEqual(
        JsonConvert.SerializeObject(fakeJobs),
        JsonConvert.SerializeObject(receivedLocations[0].Jobs)
      );
    }

    [Test]
    public async Task ReadsLocationsFromCache() {
      (
        var jobsService,
        var mockFileSystem,
        var mockConfiguration,
        var mockHttpClientFactory,
        var mockMemoryCache
      ) = CreateJobsService();

      var fakeJob = new Job {
        ID = 9999,
        Name = "Test job",
        LocationID = 99999
      };
      var fakeJobs = new List<Job> { fakeJob };
      var fakeLocations = new List<Location> {
        new Location {
          ID = 99999,
          Name = "Newcastle",
          State = "NSW",
          Jobs = fakeJobs
        }
      };

      // when mocking TryGetValue (in lieu of Get), the out parameter must be declared as an
      // object even if it isn't, if you don't do this then it will match a templated extension
      // method of the same name
      object fakeJobsObject = fakeJobs;
      object fakeLocationsObject = fakeLocations;

      mockMemoryCache.Setup(x =>
        x.TryGetValue(
          It.Is<string>(s => s == JobsService.JOBS_CACHE_KEY),
          out fakeJobsObject
        )
      ).Returns(true);

      mockMemoryCache.Setup(x =>
        x.TryGetValue(
          It.Is<string>(s => s == JobsService.LOCATIONS_CACHE_KEY),
          out fakeLocationsObject
        )
      ).Returns(true);


      var receivedLocations = (await jobsService.GetAllLocations()).ToList();


      Assert.That(receivedLocations.Count(), Is.EqualTo(1));
      Assert.That(receivedLocations[0].ID, Is.EqualTo(99999));
      Assert.That(receivedLocations[0].Name, Is.EqualTo("Newcastle"));
      Assert.That(receivedLocations[0].State, Is.EqualTo("NSW"));
      Assert.That(receivedLocations[0].Jobs.Count(), Is.EqualTo(1));

      // comparing by value
      Assert.AreEqual(
        JsonConvert.SerializeObject(fakeJobs),
        JsonConvert.SerializeObject(receivedLocations[0].Jobs)
      );
    }

    /// <summary>
    ///   Sets up the tests for the service
    /// <summary>
    /// <returns>a tuple of mocked dependencies</returns>
    private (
        JobsService,
        Mock<IFileSystem>,
        Mock<IConfiguration>,
        Mock<IHttpClientFactory>,
        Mock<IMemoryCache>
    ) CreateJobsService() {
      var services = new ServiceCollection();
      services.AddTransient<JobsService>();

      var mockFileSystem = new Mock<IFileSystem>();
      services.AddScoped<IFileSystem>(factory => mockFileSystem.Object);
      var mockConfiguration = new Mock<IConfiguration>();
      services.AddScoped<IConfiguration>(factory => mockConfiguration.Object);
      var mockHttpClientFactory = new Mock<IHttpClientFactory>();
      services.AddScoped<IHttpClientFactory>(factory => mockHttpClientFactory.Object);
      var mockMemoryCache = new Mock<IMemoryCache>();
      services.AddScoped<IMemoryCache>(factory => mockMemoryCache.Object);

      var serviceProvider = services.BuildServiceProvider();
      return (
        serviceProvider.GetService<JobsService>(),
        mockFileSystem,
        mockConfiguration,
        mockHttpClientFactory,
        mockMemoryCache
      );
    }
  }
}
