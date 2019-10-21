# RavenDB 4: Getting Started Pluralsight Course

This sample code is designed to be used alongside the Pluralsight course **[RavenDB 4: Getting Started](https://bit.ly/PSRavenDB4)**. You may also use it as a reference application for a RavenDB demo.

For course content corrections, see [ERRATA](ERRATA.md). For changes to the code, see [CHANGELOG](CHANGELOG.md).

## Useful Resources

- [http://ravendb.net](http://ravendb.net) - Raven website, documentation, downloads, etc.
- [Google Group](https://groups.google.com/forum/#!forum/ravendb) - Community Google group
- [RavenDB Book](https://github.com/ravendb/book) - RavenDB 4 Deep Dive book
- [RavenDB Workshops](http://workshops.ravendb.net) - RavenDB international in-person workshops

## Prerequisites

### Install the .NET Core 2.0 SDK

To edit and build the source code you will need to install *at least* the .NET Core 2.0 SDK. [Follow the instructions](https://www.microsoft.com/net/download/core) for your platform.

### Install RavenDB 4

You will need a local instance of RavenDB 4 running locally. [Download the appropriate 4.0 version](https://ravendb.net/downloads) for your platform. On Windows this is a zip file you can extract anywhere.

For Docker-based installations, follow the course walkthrough or see [Docker](docs/DOCKER.md) for more information.

## Running the App

Just use the following commands in a project directory:

    dotnet restore
    dotnet watch run

On first initialization, the database will be created and seeded.

## Layout

The **src/xx-** folders contain each of the course modules source code. The **final** folder is the final code that implements all the concepts covered in the course.

Each module folder has clip folders that contains the "after" code at the end of the clip. It's assumed the clip before or module before contains the starting code as if you were following along.

```bash
# The final source code
final/
# The source code at the end of module 3, clip 3
03-document-operations/03-document-store/
```

## Codebase

The sample app is using a basic ASP.NET Core Razor Pages layout.

- **Startup.cs** - The main application startup logic (wires services, configuration, etc.)
- **DocumentStoreHolder.cs** - An example singleton service that allows the code to access the Raven document store. Initialized once on the first app request.
- **Services/** - Holds the data services used in the app
    - **Services/RavenTalkService.cs** - The **main event**, this holds all the Raven data access logic
    - **Services/InMemoryTalkService.cs** - The in-memory data access service
- **Pages/** - Holds the Razor pages and code-behind of each page
- **Views/Shared/Components/** - Holds shared Razor components

I recommend using Visual Studio Code to edit and run the code.

## Bugs

If you discover a bug, please open an issue. I will record issues in the CHANGELOG.

## Caveats

This sample is intended to be a beginner's guide to Raven and as such does not reflect appropriate optimizations that you'd make for production use (consolidated indexes, optimized session handling, lazy queries, etc.). The intent was to prefer simple over complex for illustration and learning.
