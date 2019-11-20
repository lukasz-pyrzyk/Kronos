write-host "Packaging"

dotnet pack -c Release

# Set build as failed if any error occurred
if($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode )  }

write-host "Packeging finished"

write-host "Preparing docker build"

cd "Src\Kronos.Server"
dotnet publish -o ./publish -c Release
write-host "Project packaged"

docker build --no-cache -t lukaszpyrzyk/kronos .
write-host "Docker image built"

if($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode )  }
