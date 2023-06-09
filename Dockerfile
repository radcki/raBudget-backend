FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY *.sln .
COPY . .
RUN dotnet restore raBudget.sln
RUN dotnet publish raBudget.sln -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "raBudget.Api.dll"]
