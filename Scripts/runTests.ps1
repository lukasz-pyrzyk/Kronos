param (
    [Parameter(Mandatory=$true)][string]$fileName = ""
)

$openCover = Get-ChildItem -Path "C:\Users\$([Environment]::UserName)\.nuget\packages\OpenCover\" -Filter "OpenCover.Console.exe" -Recurse | % { $_.FullName }

# entry folder
$kronosSrc = ".\Src\"

# test projects to run with OpenCover
$projects = @(
    @{Path="Tests\Kronos.Core.Tests"; Filter="+[Kronos.Core]*"}
    @{Path="Tests\Kronos.Client.Tests"; Filter="+[Kronos.Client]*"}
    @{Path="Tests\Kronos.Server.Tests"; Filter="+[Kronos.Server]*"}
)

function RunCodeCoverage($testProject, $filter) {
    & $openCover -target:dotnet.exe `"-targetargs:test $testProject`" -output:$fileName -register:'user' -filter:$filter -mergeoutput
}

# run unit tests and calculate code coverage for each test project
foreach ($project in $projects) {
    RunCodeCoverage $($kronosSrc + $project.Path) $project.Filter
}

# try to find error
if($LastExitCode -ne 0)
{
   # mark build as failed
   $host.SetShouldExit($LastExitCode)
}