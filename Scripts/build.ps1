write-host "Build started"

dotnet build -c Release

# Set build as failed if any error occurred
if($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode )  }

write-host "Build finished"