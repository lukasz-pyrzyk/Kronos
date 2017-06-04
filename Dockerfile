FROM microsoft/dotnet:runtime

# maintener info
MAINTAINER Lukasz Pyrzyk <lukasz.pyrzyk@gmail.com>


## Required to correct .NET Core startup
WORKDIR /dotnetapp

# copy published binaries to the container
COPY ./Src/Kronos.Server/Bin .

# set entrypoint to the docker run
ENTRYPOINT ["dotnet", "Kronos.Server.dll"]
