# Базовый ASP.NET Core Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5050
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 5050

# Сборка приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ENV BUILD_CONFIGURATION=$BUILD_CONFIGURATION
WORKDIR /src

COPY ["MessagingApp.API/MessagingApp.API.csproj", "MessagingApp.API/"]
COPY ["MessagingApp.DAL/MessagingApp.DAL.csproj", "MessagingApp.DAL/"]
COPY ["MessagingApp.Models/MessagingApp.Models.csproj", "MessagingApp.Models/"]
RUN dotnet restore "MessagingApp.API/MessagingApp.API.csproj"

COPY . .  
WORKDIR "/src/MessagingApp.API"
RUN dotnet build "MessagingApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Публикация
FROM build AS publish
RUN dotnet publish "MessagingApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Запуск API
ENTRYPOINT ["dotnet", "MessagingApp.API.dll"]
