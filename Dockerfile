FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build

WORKDIR /app

COPY . .
RUN dotnet restore raBudget.Api/raBudget.Api.csproj
RUN dotnet publish raBudget.Api/raBudget.Api.csproj -c Release -o /app/out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:11.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "raBudget.Api.dll"]
