write-host "Tests started"

dotnet test -c Release

# Set build as failed if any error occurred
if($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode )  }

write-host "Tests finished"