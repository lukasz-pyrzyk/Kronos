dotnet restore

RUNTIME="netcoreapp1.0"
dotnet test Src/Kronos.Core/project.json -f $RUNTIME
dotnet test Src/Kronos.Client/project.json -f $RUNTIME
dotnet test Src/Kronos.Server/project.json -f $RUNTIME
dotnet test Src/Kronos.AcceptanceTest/project.json -f $RUNTIME