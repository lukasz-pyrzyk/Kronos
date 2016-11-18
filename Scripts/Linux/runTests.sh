dotnet restore

dotnet test Src/Kronos.Core/project.json -f netstandard1.3
dotnet test Src/Kronos.Client/project.json -f netstandard1.3
dotnet test Src/Kronos.Server/project.json -f netstandard1.3
dotnet test Src/Kronos.AcceptanceTest/project.json -f netstandard1.3