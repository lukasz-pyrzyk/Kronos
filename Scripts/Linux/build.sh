#!/usr/bin/env bash

#settings
MODE="Release"


#exit if any command fails
set -e

#artifactsFolder="./artifacts"

#if [ -d $artifactsFolder ]; then  
  #rm -R $artifactsFolder
#fi

dotnet restore

# Ideally we would use the 'dotnet test' command to test netcoreapp and net451 so restrict for now 
# but this currently doesn't work due to https://github.com/dotnet/cli/issues/3073 so restrict to netcoreapp

dotnet build Src/Kronos.Core/ -c $MODE 
dotnet build Src/Kronos.Client/ -c $MODE 
dotnet build Src/Kronos.Server/ -c $MODE 

#mono \  
#./test/TEST_PROJECT_NAME/bin/Release/net451/*/dotnet-test-xunit.exe \
#./test/TEST_PROJECT_NAME/bin/Release/net451/*/TEST_PROJECT_NAME.dll

revision=${TRAVIS_JOB_ID:=1}  
revision=$(printf "%04d" $revision) 

#dotnet pack ./src/PROJECT_NAME -c Release -o ./artifacts --version-suffix=$revision  