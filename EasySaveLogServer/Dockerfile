# 1. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project files to restore dependencies
COPY ["EasySaveLogServer/EasySaveLogServer.csproj", "EasySaveLogServer/"]
COPY ["EasyLog/EasyLog.csproj", "EasyLog/"]

# Restore the main project (this will also restore EasyLog via project reference)
RUN dotnet restore "EasySaveLogServer/EasySaveLogServer.csproj"

# Copy the entire solution source code
COPY . .

# Build and publish the server in Release mode
WORKDIR "/src/EasySaveLogServer"
RUN dotnet publish "EasySaveLogServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Execution Stage
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Create the Logs directory to prevent permission errors
RUN mkdir -p /app/Logs

EXPOSE 12345
ENTRYPOINT ["dotnet", "EasySaveLogServer.dll"]