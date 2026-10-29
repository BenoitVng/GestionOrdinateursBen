# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY GestionOrdinateurs.Gateway/GestionOrdinateurs.Gateway.csproj ./GestionOrdinateurs.Gateway/
RUN dotnet restore ./GestionOrdinateurs.Gateway

COPY GestionOrdinateurs.Gateway/ ./GestionOrdinateurs.Gateway/
RUN dotnet publish ./GestionOrdinateurs.Gateway -c Release -o out --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5238
EXPOSE 5238

ENTRYPOINT ["dotnet", "GestionOrdinateurs.Gateway.dll"]
