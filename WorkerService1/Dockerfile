#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WorkerService1/WorkerService1.csproj", "WorkerService1/"]
RUN dotnet restore "WorkerService1/WorkerService1.csproj"
COPY . .
WORKDIR "/src/WorkerService1"
RUN dotnet build "WorkerService1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorkerService1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkerService1.dll"]