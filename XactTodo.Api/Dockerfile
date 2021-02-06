#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["XactTodo.Api/XactTodo.Api.csproj", "XactTodo.Api/"]
COPY ["XactTodo.Infrastructure/XactTodo.Infrastructure.csproj", "XactTodo.Infrastructure/"]
COPY ["XactTodo.Domain/XactTodo.Domain.csproj", "XactTodo.Domain/"]
COPY ["XactTodo.Security/XactTodo.Security.csproj", "XactTodo.Security/"]
RUN dotnet restore "XactTodo.Api/XactTodo.Api.csproj"
COPY . .
WORKDIR "/src/XactTodo.Api"
RUN dotnet build "XactTodo.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "XactTodo.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "XactTodo.Api.dll"]