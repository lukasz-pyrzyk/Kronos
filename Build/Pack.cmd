set version-suffix=%1

cd ..\Src\
cd Kronos.Core
dotnet pack --configuration --Release --version-suffix %version-suffix%
cd ..
cd Kronos.Client
dotnet pack --configuration --Release --version-suffix %version-suffix%
cd ..
cd Kronos.Server
dotnet pack --configuration --Release --version-suffix %version-suffix%
exit