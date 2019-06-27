FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["HybridEncryptionLogin/HybridEncryptionLogin.csproj", "HybridEncryptionLogin/"]
COPY ["HybridEncryptionLogin.Services/HybridEncryptionLogin.Services.csproj", "HybridEncryptionLogin.Services/"]
RUN dotnet restore "HybridEncryptionLogin/HybridEncryptionLogin.csproj"
COPY . .
WORKDIR "/src/HybridEncryptionLogin"
RUN dotnet build "HybridEncryptionLogin.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "HybridEncryptionLogin.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "HybridEncryptionLogin.dll"]