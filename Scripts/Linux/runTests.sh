#exit if any command fails
set -e

dotnet restore

RUNTIME="netcoreapp1.1"
dotnet test Tests/Kronos.Core.Tests/Kronos.Core.Tests.csproj -f $RUNTIME
dotnet test Tests/Kronos.Client.Tests/Kronos.Client.Tests.csproj -f $RUNTIME
dotnet test Tests/Kronos.Server.Tests/Kronos.Server.Tests.csproj -f $RUNTIME
dotnet test Tests/Kronos.AcceptanceTest/Kronos.AcceptanceTest.csproj -f $RUNTIME
