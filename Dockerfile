
FROM microsoft/aspnet:latest
MAINTAINER Elton Stoneman <elton@sixeyed.com>

RUN sh -c 'echo "deb [arch=amd64] http://apt-mo.trafficmanager.net/repos/dotnet/ trusty main" > /etc/apt/sources.list.d/dotnetdev.list'
RUN apt-key adv --keyserver apt-mo.trafficmanager.net --recv-keys 417A0893
RUN apt-get update
RUN apt-get install -y dotnet

# create folders
RUN mkdir -p /Src

# copy all files
COPY /Src/Kronos.Server/ /Src/Kronos.Server/ 
COPY /Src/Kronos.Core/ /Src/Kronos.Core/
COPY ./NuGet.config /Src/NuGet.config

# set workdir
WORKDIR /Src

# test
RUN ["ls"]

# hello
RUN ["dnvm", "list"]

# restore packages
RUN ["dotnet", "restore"]

# run server
CMD cd /Kronos.Server && dotnet run
