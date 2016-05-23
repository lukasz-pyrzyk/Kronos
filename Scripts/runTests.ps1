# test projects
$projects = @(
    "Src\Tests\Kronos.Core.Tests\"
    "Src\Tests\Kronos.Client.Tests\"
    "Src\Tests\Kronos.Server.Tests\"
)

# test runner
function RunTests($path) {
    dotnet test $path
}

write-host "Tests started"
# test each project
foreach ($project in $projects){
    write-host "Testing " $project
    RunTests($project)
}
write-host "Tests finished"