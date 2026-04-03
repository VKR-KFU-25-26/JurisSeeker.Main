FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 8070

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["JurisSeeker.Web/JurisSeeker.Web.csproj", "JurisSeeker.Web/"]
COPY ["JurisSeeker.Persistence/JurisSeeker.Persistence.csproj", "JurisSeeker.Persistence/"]
COPY ["JurisSeeker.Domain/JurisSeeker.Domain.csproj", "JurisSeeker.Domain/"]
COPY ["JurisSeeker.Application/JurisSeeker.Application.csproj", "JurisSeeker.Application/"]
COPY ["JurisSeeker.Infrastructure/JurisSeeker.Infrastructure.csproj", "JurisSeeker.Infrastructure/"]
COPY ["JurisSeeker.Shared/JurisSeeker.Shared.csproj", "JurisSeeker.Shared/"]
COPY ["JurisSeeker.Kafka/JurisSeeker.Kafka.csproj", "JurisSeeker.Kafka/"]
RUN dotnet restore "JurisSeeker.Web/JurisSeeker.Web.csproj"
COPY . .
WORKDIR "/src/JurisSeeker.Web"
RUN dotnet build "./JurisSeeker.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./JurisSeeker.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JurisSeeker.Web.dll"]
