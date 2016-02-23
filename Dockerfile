FROM microsoft/aspnet:1.0.0-rc1-final-coreclr

# copy all files
COPY . /app/

# set workdir
WORKDIR /app

RUN ["dnu", "restore"]

WORKDIR /app/Src/Kronos.Server

ENTRYPOINT ["dnx", "run"]
