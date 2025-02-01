FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ["WebApp/WebApp.csproj", "WebApp/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Models/Models.csproj", "Models/"]
COPY ["Repositories/Repositories.csproj", "Repositories/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["Tests/Tests.csproj", "Tests/"]

COPY ["AnalysisEngine.Infrastructure/AnalysisEngine.Infrastructure.csproj", "AnalysisEngine.Infrastructure/"]
COPY ["AnalysisEngine.Models/AnalysisEngine.Models.csproj", "AnalysisEngine.Models/"]
COPY ["AnalysisEngine.Repositories/AnalysisEngine.Repositories.csproj", "AnalysisEngine.Repositories/"]
COPY ["AnalysisEngine.Service/AnalysisEngine.Service.csproj", "AnalysisEngine.Service/"]

RUN dotnet restore "WebApp/WebApp.csproj"

COPY . .

RUN dotnet build "WebApp/WebApp.csproj" -c Release -o /app/build

RUN dotnet publish "WebApp/WebApp.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000
EXPOSE 5001

ENTRYPOINT ["dotnet", "WebApp.dll"]
