# Kronos  [![NuGet version](https://badge.fury.io/nu/Kronos.Client.svg)](https://badge.fury.io/nu/Kronos.Client)

Kronos is distributed, in-memory cache system based on .NET Core. Client library is available by [Nuget](https://www.nuget.org/packages/Kronos.Client/).

### Build status
| Windows |  Linux |
|:-------:|:------:|
| [![Build status](https://ci.appveyor.com/api/projects/status/vrkm5pcbg0dv6n6w/branch/master?svg=true)](https://ci.appveyor.com/project/LukaszPyrzyk/kronos/branch/master) | [![Build status](https://ci.appveyor.com/api/projects/status/vrkm5pcbg0dv6n6w/branch/master?svg=true)](https://ci.appveyor.com/project/LukaszPyrzyk/kronos/branch/master) |

### Known performance issues
- Google protobuf library doesn't support async read/write operation. Because of that, reading from the network stream is synchronous what blocks the thread. https://github.com/lukasz-pyrzyk/Kronos/issues/184
- A package travels via protobuf, which always allocates new memory for the request. It means that we allocate a new array of bytes for new package. It must be changed.

### Docker Image
[![](https://images.microbadger.com/badges/version/lukaszpyrzyk/kronos.svg)](https://microbadger.com/images/lukaszpyrzyk/kronos "Kronos ") [![](https://images.microbadger.com/badges/image/lukaszpyrzyk/kronos.svg)](https://microbadger.com/images/lukaszpyrzyk/kronos "Kronos") [![Docker Stars](https://img.shields.io/docker/stars/lukaszpyrzyk/kronos.svg)](https://hub.docker.com/r/lukaszpyrzyk/kronos/)

### Kronos.Client - Installation
```csharp
$ Install-Package Kronos.Client
```

### Kronos.Client - Usage
```csharp
// initialize client
KronosClient client = KronosClientFactory.FromIp("127.0.0.1", 44000);

// create a key, package and expiry date
string key = "key";
byte[] packageToCache = Encoding.UTF8.GetBytes("Lorem ipsum");
DateTime expiryDate = DateTime.UtcNow.AddDays(5);

// insert package to server
InsertResponse insert = await client.InsertAsync(key, packageToCache, expiryDate);

// get package from server
GetResponse getResponse = await client.GetAsync(key);

// check if storage contains given key
ContainsResponse contains  = await client.ContainsAsync(key);

// count number of elements in the storage
CountResponse number  = await client.CountAsync();

// Optionally you can delete object from cache using Delete command
DeleteResponse delete = await client.DeleteAsync(key);

// or flush all storage
ClearResponse clear = await client.ClearAsync();
```

### Server initialization using docker image
To start server using docker, login as a sudo and type:
```bash
docker run -td -p 44000:44000 lukaszpyrzyk/kronos 44000
```
where: 
* t - allocate a pseudo-tty
* d - start a container in detached mode
* p - assign public port 44000 to internal 44000
* lukaszpyrzyk/kronos - name of the image, https://hub.docker.com/r/lukaszpyrzyk/kronos/
* 44000 - number of internal port, value passed to the Kronos

Full documentation is available on the [docker reference page](https://docs.docker.com/engine/reference/run/)

### Building own docker image
If you don't want to use my docker image, you can build your own. Install .NET Core 2.2, clone repository and type: 
```bash
dotnet publish Src/Kronos.Server -c Release
docker build -t kronos .
```

### University
I am a student of Opole University Of technology and this is my Engineering Thesis.

License
----
MIT