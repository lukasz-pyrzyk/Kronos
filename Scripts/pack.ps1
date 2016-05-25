param (
    [Parameter(Mandatory=$true)][string]$version = 0
)

# projects to pack
$projects = @(
    "Src\Kronos.Core",
    "Src\Kronos.Client",
    "Src\Kronos.Server"
)

# pack function for project
function Pack($path) {
    dotnet pack $path --version-suffix $version
}

write-host "Packeging started"
# build each project
foreach ($project in $projects){
    write-host "Packing " $project "with suffix version " $version
    Pack($project)
}
write-host "Packeging finished"