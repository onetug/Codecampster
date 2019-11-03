FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src

COPY scripts scripts/

COPY /src/BuildingBlocks/*/*/*.csproj /src/csproj-files/
COPY /src/Web/WebMVC/*.csproj /src/csproj-files/

RUN chmod +x scripts/restore-packages.sh && scripts/restore-packages.sh

COPY . .
WORKDIR /src/src/Web/WebMVC
RUN dotnet publish -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebMVC.dll"]