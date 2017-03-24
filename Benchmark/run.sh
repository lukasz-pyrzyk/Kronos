cd ..

# build Kronos locally and copy files to /Bin
dotnet restore
dotnet publish ./Src/Kronos.Server/ -c Release -o ./Bin -f netcoreapp1.1

# build docker image
docker build -t lukaszpyrzyk/kronos:dev .

# stop all running containers
docker stop $(docker ps -aq)

# run image
docker run -td -p 5000:5000 lukaszpyrzyk/kronos:dev

# go back to the benchmark folder
cd Benchmark

# run another containers for Benchmark, like Redis
docker-compose up -d --force-recreate

# run Benchmark
dotnet run -c Release -f netcoreapp1.1
