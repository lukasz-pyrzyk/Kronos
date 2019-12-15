FROM mcr.microsoft.com/dotnet/core/runtime:3.0

WORKDIR /app

COPY ./bin/Release/netcoreapp3.0/publish/ .

ENTRYPOINT ["dotnet", "Kronos.Server.dll"]