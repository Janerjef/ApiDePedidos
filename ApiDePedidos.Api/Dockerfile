FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ApiDePedidos.Api/ApiDePedidos.Api.csproj", "ApiDePedidos.Api/"]
COPY ["ApiDePedidos.Application/ApiDePedidos.Application.csproj", "ApiDePedidos.Application/"]
COPY ["ApiDePedidos.Domain/ApiDePedidos.Domain.csproj", "ApiDePedidos.Domain/"]
COPY ["ApiDePedidos.Infrastructure/ApiDePedidos.Infrastructure.csproj", "ApiDePedidos.Infrastructure/"]
RUN dotnet restore "ApiDePedidos.Api/ApiDePedidos.Api.csproj"
COPY . .
WORKDIR "/src/ApiDePedidos.Api"
RUN dotnet build "./ApiDePedidos.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ApiDePedidos.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiDePedidos.Api.dll"]
