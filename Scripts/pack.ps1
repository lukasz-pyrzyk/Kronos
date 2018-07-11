# projects to pack
$projects = @(
    "Src\Kronos.Core",
    "Src\Kronos.Client",
    "Src\Kronos.Server"
)

# pack function for project
function Pack($path) {
    dotnet pack $path -c Release;
}

write-host "Packeging started"
# build each project
foreach ($project in $projects){
    write-host "Packing " $project
    Pack($project)
}

# Set build as failed if any error occurred
if($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode )  }

write-host "Packeging finished"