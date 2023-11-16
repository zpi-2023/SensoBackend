FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SensoBackend/SensoBackend.csproj", "SensoBackend/"]
RUN dotnet restore "SensoBackend/SensoBackend.csproj"
COPY . .
WORKDIR "/src/SensoBackend"
RUN dotnet build "SensoBackend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SensoBackend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
RUN apt-get -y update; apt-get -y install curl
WORKDIR /app
COPY --from=publish /app/publish .
HEALTHCHECK CMD curl -f http://localhost:80/api/v1/healthz || exit 1
ENTRYPOINT ["dotnet", "SensoBackend.dll"]
