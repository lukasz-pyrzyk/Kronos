FROM microsoft/dotnet:2.2-runtime

# maintener info
MAINTAINER Lukasz Pyrzyk <lukasz.pyrzyk@gmail.com>

## Required to correct .NET Core startup
WORKDIR /dotnetapp

# copy published binaries to the container
COPY ./Src/Kronos.Server/bin/Release/ .

# set entrypoint to the docker run
ENTRYPOINT ["dotnet", "Kronos.Server.dll"]
