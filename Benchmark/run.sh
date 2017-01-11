docker-compose up -d --force-recreate
dotnet restore
dotnet run -c Release
