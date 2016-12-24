#exit if any command fails
set -e

dotnet restore

RUNTIME="netcoreapp1.1"
dotnet test Tests/Kronos.Core.Tests/ -f $RUNTIME
dotnet test Tests/Kronos.Client.Tests/ -f $RUNTIME
dotnet test Tests/Kronos.Server.Tests/ -f $RUNTIME
dotnet test Tests/Kronos.AcceptanceTest/ -f $RUNTIME