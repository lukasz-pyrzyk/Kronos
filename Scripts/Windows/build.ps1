# build function for project
write-host "Build started"
# restore packages
write-host "Restoring packages"
dotnet restore
write-host "Restoring packages finished"

write-host "Building"
dotnet build
write-host "Building finished"
	
# Set build as failed if any error occurred
if($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode )  }

write-host "Build finished"