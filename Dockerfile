FROM microsoft/dotnet:runtime

# maintener info
MAINTAINER Lukasz Pyrzyk <lukasz.pyrzyk@gmail.com>

# set workdir
WORKDIR /dotnetapp

# copy published binaries to the container
COPY ./Bin .

# set entrypoint to the docker run
ENTRYPOINT ["dotnet", "Kronos.Server.dll"]
