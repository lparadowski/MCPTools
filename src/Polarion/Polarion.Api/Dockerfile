FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Polarion/ Polarion/
COPY Shared/ Shared/
RUN dotnet publish Polarion/Polarion.Api/Polarion.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
COPY --from=build /app/publish /app
WORKDIR /app
EXPOSE 8080
ENTRYPOINT ["dotnet", "Polarion.Api.dll"]
