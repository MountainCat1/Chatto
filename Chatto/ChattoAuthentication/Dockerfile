﻿FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8001
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ChattoAuth/ChattoAuth.csproj", "ChattoAuth/"]
RUN dotnet restore "ChattoAuth/ChattoAuth.csproj"
COPY . .
WORKDIR "/src/ChattoAuth"
RUN dotnet build "ChattoAuth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChattoAuth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChattoAuth.dll"]
