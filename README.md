# Kronos  [![Build status](https://ci.appveyor.com/api/projects/status/26qj17kq09btkkql?svg=true)](https://ci.appveyor.com/project/LukaszPyrzyk/binaryformatter) [![codecov.io](https://codecov.io/github/lukasz-pyrzyk/Kronos/coverage.svg?branch=master)](https://codecov.io/github/lukasz-pyrzyk/Kronos?branch=master) [![NuGet version](https://badge.fury.io/nu/Kronos.Client.svg)](https://badge.fury.io/nu/Kronos.Client)
### Description
Kronos is distributed, in-memory cache system based on .NET Core. Client library is available by [Nuget](https://www.nuget.org/packages/Kronos.Client/).

### Kronos.Client - Installation
```csharp
$ Install-Package Kronos.Client -Pre
```

### Kronos.Client - Configuration file
Kronos client requires a cluster configuration file in JSON format. Example file is assigned to the package:
```json
{
    "ClusterConfig": {
        "Servers": [
            {
                "Ip": "192.168.0.1",
                "Port": 5000
            },
            {
                "Ip": "192.168.0.1",
                "Port": 5001
            },
            {
                "Ip": "192.168.0.1",
                "Port": 5002
            }
        ]
    }
}
```

### Kronos.Client - Usage
```csharp
// initialize client
string configPath = "KronosConfig.json";
IKronosClient client = KronosClientFactory.CreateClient(configPath);

/// create a key, package and expiry date
string key = "key";
byte[] packageToCache = Encoding.UTF8.GetBytes("Lorem ipsum");
DateTime expiryDate = DateTime.Now;

// insert package to server
client.Insert(key, packageToCache, expiryDate);

// get package from server
byte[] cachedValues = client.Get(key);

// Optionally you can delete object from cache using Delete command
client.Delete(key);
```

### Server initialization using docker image
To start server using docker, login as a sudo and type:
```bash
docker run -td -p 5000:5000 lukaszpyrzyk/kronos 5000
docker run -td -p 5001:5001 lukaszpyrzyk/kronos 5001
docker run -td -p 5002:5002 lukaszpyrzyk/kronos 5002
```
where: 
* t - allocate a pseudo-tty
* d - start a container in detached mode
* p - assign public port 5000 to internal 5000
* lukaszpyrzyk/kronos - name of the image, https://hub.docker.com/r/lukaszpyrzyk/kronos/
* 5000 - number of internal port, value passed to the Kronos

Full documentation is available on the [docker reference page](https://docs.docker.com/engine/reference/run/)

### Cluster initialization using docker-compose
Create a docker-compose.yml file with cluster configuration:
```yaml
kronos-a:
  image: lukaszpyrzyk/kronos
  command: "5000"
  ports:
    - 5000:5000

kronos-b:
  image: lukaszpyrzyk/kronos
  command: "5001"
  ports:
    - 5001:5001

kronos-c:
  image: lukaszpyrzyk/kronos
  command: "5002"
  ports:
    - 5002:5002
```
Save the file and run command:
```bash
docker-compose up -d
```
where 
* d - start a containers in detached mode

This command will create and start three Kronos Severs.

### Building own docker image
If you don't want to use my docker image, you can build your own. Clone repository and type: 
```bash
docker build -t kronos .
```

### University
I am a student of Opole University Of technology and this is my Engineering Thesis.

License
----
MIT

   [kronos-nuget]: <https://www.nuget.org/packages/Kronos.Client/>
   [protobuf-net-url]: <https://github.com/mgravell/protobuf-net>
