FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Artifactory/ Artifactory/
COPY Shared/ Shared/
RUN dotnet publish Artifactory/Artifactory.Api/Artifactory.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
COPY --from=build /app/publish /app
WORKDIR /app
EXPOSE 8080
ENTRYPOINT ["dotnet", "Artifactory.Api.dll"]
