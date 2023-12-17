#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/ApiGateway/ErSoftDev.ApiGateway/ErSoftDev.ApiGateway.csproj", "src/ApiGateway/ErSoftDev.ApiGateway/"]
COPY ["src/ErSoftDev.Framework/ErSoftDev.Framework.csproj", "src/ErSoftDev.Framework/"]
COPY ["src/ErSoftDev.Common/ErSoftDev.Common.csproj", "src/ErSoftDev.Common/"]
COPY ["src/ErSoftDev.DomainSeedWork/ErSoftDev.DomainSeedWork.csproj", "src/ErSoftDev.DomainSeedWork/"]
RUN dotnet restore "src/ApiGateway/ErSoftDev.ApiGateway/ErSoftDev.ApiGateway.csproj"
COPY . .
WORKDIR "/src/src/ApiGateway/ErSoftDev.ApiGateway"
RUN dotnet build "ErSoftDev.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ErSoftDev.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ErSoftDev.ApiGateway.dll"]