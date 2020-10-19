# FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
# WORKDIR /app

# Copy csproj and restore as distinct layers
# COPY Pellio/Pellio.sln ./Pellio/
# COPY Pellio/*.csproj ./Pellio/
# WORKDIR  /app/Pellio
# RUN dotnet restore

# Copy everything else and build
# COPY Pellio/. ./Pellio/
# COPY UnitTest/. ./UnitTest/
# WORKDIR  /app/Pellio
# RUN dotnet publish -c Release -o out

# Build runtime image
# FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
# WORKDIR /app/Pellio
# COPY --from=build /app/out ./
# ENTRYPOINT ["dotnet", "Pellio.dll"]

#               aasdasdas
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY Pellio/Pellio.sln ./Pellio/
COPY Pellio/*.csproj ./Pellio/
WORKDIR /source/Pellio
RUN dotnet restore

# copy everything else and build app
WORKDIR /source
COPY Pellio/. ./Pellio/
WORKDIR /source/Pellio
RUN dotnet publish -c release -o /app --no-restore 

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Pellio.dll"]