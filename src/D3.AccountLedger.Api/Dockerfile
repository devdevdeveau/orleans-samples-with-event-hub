#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/D3.AccountLedger.Api/D3.AccountLedger.Api.csproj", "src/D3.AccountLedger.Api/"]
RUN dotnet restore "src/D3.AccountLedger.Api/D3.AccountLedger.Api.csproj"
COPY . .
WORKDIR "/src/src/D3.AccountLedger.Api"
RUN dotnet build "D3.AccountLedger.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "D3.AccountLedger.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "D3.AccountLedger.Api.dll"]