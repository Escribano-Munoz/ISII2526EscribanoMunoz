FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["AppForSEII2526.LogViewer.csproj", "./"]
RUN dotnet restore "AppForSEII2526.LogViewer.csproj"

COPY . .
RUN dotnet build "AppForSEII2526.LogViewer.csproj" -c Release -o /app/build

RUN dotnet publish "AppForSEII2526.LogViewer.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AppForSEII2526.LogViewer.dll"]