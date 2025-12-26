FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

RUN dotnet tool install --global dotnet-ef --version 8.0.8
ENV PATH="$PATH:/root/.dotnet/tools"

WORKDIR /app

COPY src/ServiceOrders.Api/ServiceOrders.Api.csproj src/ServiceOrders.Api/
COPY src/ServiceOrders.Application/ServiceOrders.Application.csproj src/ServiceOrders.Application/
COPY src/ServiceOrders.Domain/ServiceOrders.Domain.csproj src/ServiceOrders.Domain/
COPY src/ServiceOrders.Infrastructure/ServiceOrders.Infrastructure.csproj src/ServiceOrders.Infrastructure/

RUN dotnet restore src/ServiceOrders.Api/ServiceOrders.Api.csproj

COPY . .

RUN dotnet publish src/ServiceOrders.Api/ServiceOrders.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "ServiceOrders.Api.dll"]
