FROM dockerhost.charisma.tech/dotnet/aspnet:7.0 AS base
WORKDIR /app
COPY ./app /app
EXPOSE 80
ENTRYPOINT ["dotnet", "Charisma.DevelopmentTools.Endpoint.Api.dll"]