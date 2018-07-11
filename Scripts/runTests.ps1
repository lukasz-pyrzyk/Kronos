# test projects to run with OpenCover
$projects = @(
    @{Path="Tests\Kronos.Core.Tests\Kronos.Core.Tests.csproj"}
    @{Path="Tests\Kronos.Client.Tests\Kronos.Client.Tests.csproj"}
    @{Path="Tests\Kronos.Server.Tests\Kronos.Server.Tests.csproj"}
    @{Path="Tests\Kronos.AcceptanceTest\Kronos.AcceptanceTest.csproj"}
)

$error = 0;

function RunTests($testProject) {
    write-host "Running tests for " $testProject
    dotnet test $testProject
}

# run unit tests and calculate code coverage for each test project
foreach ($project in $projects) {
    RunTests $($project.Path)

	# try to find error
	if($LastExitCode -ne 0)
	{
	   write-host "Setting error"
	   # mark build as failed
	   $error = $LastExitCode
	}
}

# try to find error
if($error -ne 0)
{
   write-host "Marking build as failed"
   # mark build as failed
   $host.SetShouldExit($error)
}
