FROM mcr.microsoft.com/dotnet/core/runtime:3.0

WORKDIR /app

COPY ./publish .

ENTRYPOINT ["dotnet", "Kronos.Server.dll"]