FROM dockerhost.charisma.tech/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY ./app /app
EXPOSE 80
ENTRYPOINT ["dotnet", "CPG.API.dll"]