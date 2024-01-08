#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Identity/ErSoftDev.Identity.EndPoint/ErSoftDev.Identity.EndPoint.csproj", "src/Identity/ErSoftDev.Identity.EndPoint/"]
COPY ["src/Identity/ErSoftDev.Identity.Application/ErSoftDev.Identity.Application.csproj", "src/Identity/ErSoftDev.Identity.Application/"]
COPY ["src/Identity/ErSoftDev.Identity.Domain/ErSoftDev.Identity.Domain.csproj", "src/Identity/ErSoftDev.Identity.Domain/"]
COPY ["src/ErSoftDev.DomainSeedWork/ErSoftDev.DomainSeedWork.csproj", "src/ErSoftDev.DomainSeedWork/"]
COPY ["src/Identity/ErSoftDev.Identity.Infrastructure/ErSoftDev.Identity.Infrastructure.csproj", "src/Identity/ErSoftDev.Identity.Infrastructure/"]
COPY ["src/ErSoftDev.Framework/ErSoftDev.Framework.csproj", "src/ErSoftDev.Framework/"]
COPY ["src/ErSoftDev.Common/ErSoftDev.Common.csproj", "src/ErSoftDev.Common/"]
RUN dotnet restore "./src/Identity/ErSoftDev.Identity.EndPoint/ErSoftDev.Identity.EndPoint.csproj"
COPY . .
WORKDIR "/src/src/Identity/ErSoftDev.Identity.EndPoint"
RUN dotnet build "./ErSoftDev.Identity.EndPoint.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ErSoftDev.Identity.EndPoint.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ErSoftDev.Identity.EndPoint.dll"]