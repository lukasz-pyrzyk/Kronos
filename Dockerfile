FROM microsoft/dotnet:runtime

# Required to correct .NET Core startup
WORKDIR /Kronos

# maintener info
MAINTAINER Lukasz Pyrzyk <lukasz.pyrzyk@gmail.com>

# copy published binaries to the container
COPY ./Bin .

# set entrypoint to the docker run
ENTRYPOINT ["dotnet", "Kronos.Server.dll"]
