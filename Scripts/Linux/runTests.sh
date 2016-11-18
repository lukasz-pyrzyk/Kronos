dotnet restore

RUNTIME="netcoreapp1.0"
dotnet test Tests/Kronos.Core.Tests/project.json -f $RUNTIME
dotnet test Tests/Kronos.Client.Tests/project.json -f $RUNTIME
dotnet test Tests/Kronos.Server.Tests/project.json -f $RUNTIME
dotnet test Tests/Kronos.AcceptanceTest/project.json -f $RUNTIME