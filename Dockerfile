FROM microsoft/dotnet:1.0.0-preview1

# copy all files
COPY . /app/

# set workdir
WORKDIR /app

RUN ["dotnet", "restore"]

WORKDIR /app/Src/Kronos.Server

ENTRYPOINT ["dotnet", "run"]
