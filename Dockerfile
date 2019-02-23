FROM microsoft/dotnet:2.2-runtime
WORKDIR /app
ENTRYPOINT ["dotnet", "Kronos.Server.dll"]
COPY Src/Kronos.Server/bin/Release/netcoreapp2.2 .