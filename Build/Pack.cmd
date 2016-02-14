set version-suffix=%1

cd ..\Src\
cd Kronos.Core
dotnet pack --configuration --Release --version-suffix %version-suffix% -o ..\..\Build\Packages
cd ..
cd Kronos.Client
dotnet pack --configuration --Release --version-suffix %version-suffix% -o ..\..\Build\Packages
cd ..
cd Kronos.Server
dotnet pack --configuration --Release --version-suffix %version-suffix% -o ..\..\Build\Packages
exit