FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Trello/ Trello/
COPY Shared/ Shared/
RUN dotnet publish Trello/Trello.Api/Trello.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
COPY --from=build /app/publish /app
WORKDIR /app
EXPOSE 8080
ENTRYPOINT ["dotnet", "Trello.Api.dll"]
