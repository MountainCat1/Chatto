FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Chatto/Chatto.csproj", "Chatto/"]
RUN dotnet restore "Chatto/Chatto.csproj"
COPY . .
WORKDIR "/src/Chatto"
RUN dotnet build "Chatto.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Chatto.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chatto.dll"]
