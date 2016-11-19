FROM microsoft/dotnet

# maintener info
MAINTAINER Lukasz Pyrzyk <lukasz.pyrzyk@gmail.com>

# copy all files
COPY ./Src/Kronos.Core /app/Kronos.Core
COPY ./Src/Kronos.Server /app/Kronos.Server
COPY ./NuGet.config /app/

# set workdir
WORKDIR /app

# restore nuget packages
RUN dotnet restore

WORKDIR Kronos.Server

# compile with release mode
RUN dotnet build --framework netcoreapp1.1 -c Release -o ./bin

# set entrypoint to the docker run
ENTRYPOINT ["dotnet", "./bin/Kronos.Server.dll"]
