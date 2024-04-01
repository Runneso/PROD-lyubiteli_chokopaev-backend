FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
<<<<<<< HEAD
EXPOSE 80
=======
EXPOSE 8000
EXPOSE 443
>>>>>>> 7fbd5a1 (Update events)

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . /src
RUN dotnet restore --runtime linux-x64
COPY . .
WORKDIR /src
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway.dll"]