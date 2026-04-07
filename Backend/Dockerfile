# === Étape 1 : Build (compilation) ===
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copie seulement le csproj d'abord → optimisation du cache Docker
COPY ["Backend.csproj", "./"]
RUN dotnet restore "Backend.csproj"

# Copie tout le code et publie
COPY . .
RUN dotnet publish "Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# === Étape 2 : Image finale (léger) ===
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# === Configuration ===
EXPOSE 8085
ENV ASPNETCORE_URLS=http://+:8085
ENV ASPNETCORE_ENVIRONMENT=Development

# Optionnel mais utile en dev
ENV DOTNET_RUNNING_IN_CONTAINER=true

ENTRYPOINT ["dotnet", "Backend.dll"]