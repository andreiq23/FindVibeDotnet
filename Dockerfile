# Use the official image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80 8080 8000

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore .NET dependencies
COPY ["findVibedotnet.csproj", "."]
RUN dotnet restore "findVibedotnet.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src"
RUN dotnet build "findVibedotnet.csproj" -c Release -o /app/build

# Publish the app project
FROM build AS publish
RUN dotnet publish "findVibedotnet.csproj" -c Release -o /app/publish

# Copy the build app to the base image and define entrypoint
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "findVibedotnet.dll"]
