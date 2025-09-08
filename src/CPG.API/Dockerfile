FROM dockerhost.charisma.tech/dotnet/cpg-api:1.0.0 AS base
WORKDIR /app
COPY ./app /app
EXPOSE 80
ENTRYPOINT ["dotnet", "CPG.API.dll"]