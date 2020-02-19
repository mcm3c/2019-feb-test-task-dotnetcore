## Test Task (Feb 2019)

### Tech Stack

* React (https://github.com/mcm3c/2019-feb-test-task-react)
* .NET Core (this repository)

### Task requirements

*Scenario:*

nibis revamping their Careers page to attract new employees and allow prospectiv eemployees to find the roles which they wish to apply for. The designer in your team has provided a design for the Careers page with variations at mobile, tablet and desktop resolutions. As the team’s developer you are required to implement the page. (Feel free to use any frameworks or tools you’re comfortable with).

*Challenge constraints:*

* The jobs must be filtered and sorted on the server (imagine it’s a large database of jobs, but inreality, it’s a small JSON file. You can define the format of the JSON file).
* The jobs must be fetched via AJAX on the client.
* Location identifiers are provided in the JSON file and must be joined to data retrieved from the following locations API https://private-8dbaa-nibdevchallenge.apiary-mock.com/location. Documentation for the API can be found here: https://swaggerhub.com/apis/nib-dev-challenge/Locations/1.0.0.
* Location data must be retrieved on the server
* The JSON file must not be changed on disk.
* Display only the first 130 chars of the job description.
* A user will filter the results by selecting a location, e.g. Sydney.
* Then a user will select a job and be taken to a full job description page.
* When a user clicks back in their browser, they will return to the previous screen. The previous screen will have the same filter option selected eg. Sydney.
* You will be required to commit to a GitHub git repository (This will be your personal GitHub repository) periodically throughout the challenge.
* Usage of boilerplate templates is not allowed
* The server must be written in .NET or .NET Core
* Write at least one automated test either for the server or the client


## Decisions taken

* .NET Core - The server is written in .NET Core 3.1
* Heroku - The application is containerized and running on Heroku (can be accessed here: https://dotnet-react-test-task.herokuapp.com/odata/$metadata)
* OData - The server uses OData for the sake of not reimplementing the filtering mechanism, relations, limits and so on on my own
* Tests - There are a few trivial tests written in NUnit, although I've not touched tests in .NET for quite a while and they are not that great

## Useful commands

```
Building inside a container:
$ docker build . -t dotnet-react-test-task-build -f Build.Dockerfile
$ docker run -v ${PWD}:/opt/test-task dotnet-react-test-task-build

Running this container:
$ docker build . -t dotnet-react-test-task -f Dockerfile
$ docker run dotnet-react-test-task

Deployment to heroku:
$ heroku container:push web
$ heroku container:release web
```

