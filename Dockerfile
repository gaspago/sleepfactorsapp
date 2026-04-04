# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["SleepFactorsApp.csproj", "./"]
RUN dotnet restore "SleepFactorsApp.csproj"

COPY . .
RUN dotnet build "SleepFactorsApp.csproj" -c Release -o /app/build

RUN dotnet publish "SleepFactorsApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "SleepFactorsApp.dll"]
