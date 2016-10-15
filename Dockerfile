FROM microsoft/dotnet:1.0.0-preview2-sdk

# maintener info
MAINTAINER Lukasz Pyrzyk <lukasz.pyrzyk@gmail.com>

# copy all files
COPY . /app/

# set workdir
WORKDIR /app

# restore nuget packages
RUN ["dotnet", "restore"]

# go to the server folder
WORKDIR /app/Src/Kronos.Server

# set entrypoint to the docker run
ENTRYPOINT ["dotnet", "run"]
