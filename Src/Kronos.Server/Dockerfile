FROM microsoft/dotnet:2.2-runtime

WORKDIR /app

COPY ./publish .

ENTRYPOINT ["dotnet", "Kronos.Server.dll"]