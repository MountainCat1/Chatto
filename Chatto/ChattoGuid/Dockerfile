FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8005
EXPOSE 5005

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GuidAPI/GuidAPI.csproj", "GuidAPI/"]
RUN dotnet restore "GuidAPI/GuidAPI.csproj"
COPY . .
WORKDIR "/src/GuidAPI"
RUN dotnet build "GuidAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GuidAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GuidAPI.dll"]
