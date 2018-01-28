
# Docker Configuration

On Linux/Mac, I recommend using [the Docker container](https://hub.docker.com/r/ravendb/ravendb/) and running it locally. [Kitematic](https://kitematic.com/) provides an easy to use interface to download the official RavenDB container and get it up and running in a few clicks with the settings below.

**NOTE:** On Windows, the native exe is very stable and can be run as a Windows Service. However, using Docker for Windows will also work and let you run a container that you can use for development.

- The exposed port should be 8080 (*Hostname / Ports* in Kitematic)
- The `PUBLIC_SERVER_URL` env variable should be `http://localhost:8080`
- The `UNSECURED_ACCESS_ALLOWED` env variable should be `PublicNetwork`

The following command should work once you have Docker installed:

```
docker run -d \
  --name ravendb \
  -e UNSECURED_ACCESS_ALLOWED=PublicNetwork \
  -e PUBLIC_SERVER_URL=http://localhost:8080 \
  -p 8080:8080 \
  -p 38888:38888 \
  ravendb/ravendb:latest
```

## Persisting Data

You can also mount `/databases` to a local folder on the host, like `C:\Raven` on Windows or `/var/ravendb` if you want to persist the databases.

Add this switch to the command above (replace the first path before the `:` (on Windows `C:\Raven` becomes `//c/Raven`):

```
-v //c/Raven:/databases

or

-v /var/ravendb:/databases
```
