# Kronos  [![Build status](https://ci.appveyor.com/api/projects/status/26qj17kq09btkkql?svg=true)](https://ci.appveyor.com/project/LukaszPyrzyk/binaryformatter) [![codecov.io](https://codecov.io/github/lukasz-pyrzyk/Kronos/coverage.svg?branch=master)](https://codecov.io/github/lukasz-pyrzyk/Kronos?branch=master) [![NuGet version](https://badge.fury.io/nu/Kronos.Client.svg)](https://badge.fury.io/nu/Kronos.Client)
### Description
Kronos is a byte in-memory cache system, based on .NET Core. Client library is available in [Nuget](https://www.nuget.org/packages/Kronos.Client/).

### University
I am a student of Opole University Of technology and this is my Engineering Thesis.

### Kronos.Server - Installation via Docker
```bash
docker build -t kronos .
docker run -td -p 5000:5000 kronos
```
where 5000 is port. Good job - Kronos is working!

### Kronos.Client - Installation
```csharp
$ Install-Package Kronos.Client -Pre
```

### Kronos.Client - Usage
```csharp
using (IKronosClient client = KronosClientFactory.CreateClient(IPAddress.Parse("192.168.0.2"), 5000))
{
    string key = "key";
    byte[] value = Encoding.UTF8.GetBytes("Lorem ipsum");
    DateTime expiryDate = DateTime.Now;
    client.InsertToServer(key, value, expiryDate);

    byte[] fromValue = client.TryGetValue(key);
    string valueFromServer = Encoding.UTF8.GetString(fromValue);
}
```
License
----
MIT

   [kronos-nuget]: <https://www.nuget.org/packages/Kronos.Client/>
   [protobuf-net-url]: <https://github.com/mgravell/protobuf-net>