FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8000
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Chatto.csproj", ""]
RUN dotnet restore "./Chatto.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Chatto.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Chatto.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chatto.dll"]
