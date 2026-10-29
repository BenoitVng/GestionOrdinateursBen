
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY GestionOrdinateurs.Client/GestionOrdinateurs.Client.csproj ./GestionOrdinateurs.Client/
RUN dotnet restore ./GestionOrdinateurs.Client

COPY GestionOrdinateurs.Client/ ./GestionOrdinateurs.Client/
RUN dotnet publish ./GestionOrdinateurs.Client -c Release -o out --no-restore

# Runtime 
FROM nginx:alpine AS runtime
COPY --from=build /app/out/wwwroot /usr/share/nginx/html
COPY GestionOrdinateurs.Client/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 5000
