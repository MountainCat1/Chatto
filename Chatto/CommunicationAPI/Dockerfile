FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CommunicationAPI/CommunicationAPI.csproj", "CommunicationAPI/"]
RUN dotnet restore "CommunicationAPI/CommunicationAPI.csproj"
COPY . .
WORKDIR "/src/CommunicationAPI"
RUN dotnet build "CommunicationAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CommunicationAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CommunicationAPI.dll"]
