dotnet publish -c Release
docker build -t ateliware-amazonia:latest .
docker-compose up -d --force-recreate --remove-orphans