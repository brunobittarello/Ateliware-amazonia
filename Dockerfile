FROM mcr.microsoft.com/dotnet/aspnet:6.0
EXPOSE 8080

COPY bin/Release/net6.0/publish/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "Ateliware-amazonia.dll"]
