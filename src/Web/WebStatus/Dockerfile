FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src

COPY scripts scripts/

COPY src/Web/WebStatus/*.csproj /src/csproj-files/

RUN chmod +x scripts/restore-packages.sh && scripts/restore-packages.sh

COPY . .
WORKDIR /src/src/Web/WebStatus
RUN dotnet publish -c Release -o /app

FROM build AS publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebStatus.dll"]