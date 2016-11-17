$projects = @(
    "Tests\Kronos.AcceptanceTest\"
)

# test function for project
function RunTests($path) {
    dotnet test $path -parallel none
}

function RestorePackages(){
    dotnet restore
}

write-host "Acceptance tests started"

# restore packages
write-host "Restoring packages"
RestorePackages

# run tests
foreach ($project in $projects){
    write-host "Testing " $project
    RunTests($project)
}

# Set build as failed if any error occurred
if($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode )  }

write-host "Acceptance tests finished"