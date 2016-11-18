dotnet restore

RUNTIME="netcoreapp1.0"
dotnet test Tests/Kronos.Core/project.json -f $RUNTIME
dotnet test Tests/Kronos.Client/project.json -f $RUNTIME
dotnet test Tests/Kronos.Server/project.json -f $RUNTIME
dotnet test Tests/Kronos.AcceptanceTest/project.json -f $RUNTIME