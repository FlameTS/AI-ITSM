# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first for better layer caching
COPY AIITSM.slnx ./
COPY AIITSM.Domain/AIITSM.Domain.csproj AIITSM.Domain/
COPY AIITSM.Application/AIITSM.Application.csproj AIITSM.Application/
COPY AIITSM.Infrastructure/AIITSM.Infrastructure.csproj AIITSM.Infrastructure/
COPY AIITSM.Web/AIITSM.Web.csproj AIITSM.Web/

RUN dotnet restore AIITSM.Web/AIITSM.Web.csproj

# Copy the rest of the source and publish
COPY . .
RUN dotnet publish AIITSM.Web/AIITSM.Web.csproj -c Release -o /app/publish --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Render sets $PORT at runtime; default to 8080 for local docker run
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "AIITSM.Web.dll"]
