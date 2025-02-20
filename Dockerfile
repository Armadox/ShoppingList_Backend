# 1. Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the project file (API.csproj) from the root directory
COPY ["API.csproj", "./"]

# Restore dependencies
RUN dotnet restore "API.csproj"

# Copy the rest of your source code
COPY . .

# Build the app (in Release mode, output to /app/build)
RUN dotnet build "API.csproj" -c Release -o /app/build

# 2. Publish the app (output to /app/publish)
FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish

# 3. Set up the final image, using the .NET runtime to run the app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Copy the published output from the 'publish' stage to the runtime image
COPY --from=publish /app/publish .

# Set the entry point for the container (this will run the app)
ENTRYPOINT ["dotnet", "API.dll"]
