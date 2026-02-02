FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY VideoGameExchange.Server/VideoGameExchange.Server.csproj VideoGameExchange.Server/
RUN dotnet restore VideoGameExchange.Server/VideoGameExchange.Server.csproj

COPY VideoGameExchange.Server/ VideoGameExchange.Server/
WORKDIR /src/VideoGameExchange.Server
RUN dotnet build VideoGameExchange.Server.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish VideoGameExchange.Server.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "VideoGameExchange.Server.dll"]
# Studying refrences and assistance from Claud Sonnet 4.5. Claud used for verifying correctness and debugging.