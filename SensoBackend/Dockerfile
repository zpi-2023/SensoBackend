FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SensoBackend/SensoBackend.csproj", "SensoBackend/"]
RUN dotnet restore "SensoBackend/SensoBackend.csproj"
COPY . .
WORKDIR "/src/SensoBackend"
RUN dotnet build "SensoBackend.csproj" -c Release -o /app/build

#FROM mcr.microsoft.com/dotnet/sdk:7.0 AS migration
#WORKDIR /src
#COPY ["SensoBackend/SensoBackend.csproj", "SensoBackend/"]
#RUN dotnet restore "SensoBackend/SensoBackend.csproj"
#COPY . .
#WORKDIR "/src/SensoBackend.Migration"
#RUN dotnet build "SensoBackend.Migration.csproj" -c Release -o /app/migration

FROM build AS publish
RUN dotnet publish "SensoBackend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
#WORKDIR /migration
#COPY --from=migration /app/migration .

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SensoBackend.dll"]