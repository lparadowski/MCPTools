FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY GitHub/ GitHub/
COPY Shared/ Shared/
RUN dotnet publish GitHub/GitHub.Api/GitHub.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
COPY --from=build /app/publish /app
WORKDIR /app
EXPOSE 8080
ENTRYPOINT ["dotnet", "GitHub.Api.dll"]
