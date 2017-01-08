param (
    [Parameter(Mandatory=$true)][string]$version = 0
)

# projects to pack
$projects = @(
    "Src\Kronos.Core\Kronos.Core.csproj",
    "Src\Kronos.Client\Kronos.Client.csproj",
    "Src\Kronos.Server\Kronos.Server.csproj"
)

# pack function for project
function Pack($path) {
    dotnet pack $path -c Release --version-suffix $version;
}

write-host "Packeging started"
# build each project
foreach ($project in $projects){
    write-host "Packing " $project "with suffix version " $version
    Pack($project)
}

# Set build as failed if any error occurred
if($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode )  }

write-host "Packeging finished"