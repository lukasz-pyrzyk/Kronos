#exit if any command fails
set -e

dotnet restore

MODE="Release"
RUNTIME="netcoreapp1.1"

dotnet test Tests/Kronos.Core.Tests/Kronos.Core.Tests.csproj -c $MODE -f $RUNTIME
dotnet test Tests/Kronos.Client.Tests/Kronos.Client.Tests.csproj -c $MODE -f $RUNTIME
dotnet test Tests/Kronos.Server.Tests/Kronos.Server.Tests.csproj -c $MODE -f $RUNTIME
dotnet test Tests/Kronos.AcceptanceTest/Kronos.AcceptanceTest.csproj -c $MODE -f $RUNTIME
