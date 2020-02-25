#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.1-alpine3.10 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1.101-alpine3.10 AS build
WORKDIR /src
COPY ["src/Web/CodeCampster.Web/CodeCampster.Web.csproj", "src/Web/CodeCampster.Web/"]
RUN dotnet restore "src/Web/CodeCampster.Web/CodeCampster.Web.csproj"
COPY . .
WORKDIR "/src/src/Web/CodeCampster.Web"
RUN dotnet build "CodeCampster.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodeCampster.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeCampster.Web.dll"]