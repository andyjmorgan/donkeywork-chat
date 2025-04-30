FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["src/backend/DonkeyWork.Chat.Api/DonkeyWork.Chat.Api.csproj", "src/backend/DonkeyWork.Chat.Api/"]
COPY ["src/lib/DonkeyWork.Chat.Logging/DonkeyWork.Chat.Logging.csproj", "src/lib/DonkeyWork.Chat.Logging/"]
COPY ["src/lib/DonkeyWork.Chat.AiServices/DonkeyWork.Chat.AiServices.csproj", "src/lib/DonkeyWork.Chat.AiServices/"]
COPY ["src/lib/DonkeyWork.Chat.AiTooling/DonkeyWork.Chat.AiTooling.csproj", "src/lib/DonkeyWork.Chat.AiTooling/"]
COPY ["src/lib/DonkeyWork.Chat.Common/DonkeyWork.Chat.Common.csproj", "src/lib/DonkeyWork.Chat.Common/"]
COPY ["src/lib/DonkeyWork.Chat.Providers/DonkeyWork.Chat.Providers.csproj", "src/lib/DonkeyWork.Chat.Providers/"]
COPY ["src/lib/DonkeyWork.Chat.Api.Core/DonkeyWork.Chat.Api.Core.csproj", "src/lib/DonkeyWork.Chat.Api.Core/"]
COPY ["src/persistence/DonkeyWork.Persistence.User/DonkeyWork.Persistence.User.csproj", "src/persistence/DonkeyWork.Persistence.User/"]
COPY ["src/persistence/DonkeyWork.Persistence.Common/DonkeyWork.Persistence.Common.csproj", "src/persistence/DonkeyWork.Persistence.Common/"]
COPY ["src/backend/DonkeyWork.Chat.McpServer/DonkeyWork.Chat.McpServer.csproj", "src/backend/DonkeyWork.Chat.McpServer/"]
COPY ["src/persistence/DonkeyWork.Persistence.Agent/DonkeyWork.Persistence.Agent.csproj", "src/persistence/DonkeyWork.Persistence.Agent/"]
COPY ["src/persistence/DonkeyWork.Persistence.Chat/DonkeyWork.Persistence.Chat.csproj", "src/persistence/DonkeyWork.Persistence.Chat/"]
RUN dotnet restore "src/backend/DonkeyWork.Chat.Api/DonkeyWork.Chat.Api.csproj"
COPY . .
WORKDIR "/src/src/backend/DonkeyWork.Chat.Api"
RUN dotnet build "DonkeyWork.Chat.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DonkeyWork.Chat.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DonkeyWork.Chat.Api.dll"]