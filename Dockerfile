# Build de la API (solo api/, el desktop no entra en la imagen)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia el árbol de la solución de la API y restaura
COPY api/ ./api/
RUN dotnet restore api/USystem.Api/USystem.Api.csproj

# Publica en Release
RUN dotnet publish api/USystem.Api/USystem.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

# Railway inyecta PORT; si no existe, se usa 8080
EXPOSE 8080
CMD ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet USystem.Api.dll"]
